using UnityEngine;
using Mx.Util;
using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mx.Log
{
    /// <summary>管理日记的开启和关闭</summary>
    public class DebugManager : MonoSingleton<DebugManager>
    {
        #region 数据声明
       
        private static Socket socket;
        private static IPEndPoint iPEndPoint;
        private byte[] data;

        #endregion

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Application.logMessageReceived += HandleLog;

            InitSocket();
            Debug.Log(GetType() + "Awake()/ open debug, outPath:" + Application.persistentDataPath + "/Debug/");
        }


        /// <summary>
        /// Debug回调
        /// </summary>
        /// <param name="condition">日记状况</param>
        /// <param name="stackTrace">日记堆栈跟踪</param>
        /// <param name="type">日记类型</param>
        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            string time = DateTime.Now.ToString("yyy-MM-dd HH:mm");

            if (type == LogType.Error || type == LogType.Exception)
            {
                string message = string.Format("Error:{0}\nTime:{1}\n{2}", condition, time, stackTrace);
                SaveErrorLog(message);
            }

            if (DebugDefine.IsDebugMode)
            {
                DebugData debugData = new DebugData();
                debugData.ID = stackTrace + GetTimeStamp();
                debugData.Condition = condition;
                debugData.StackTrace = stackTrace;
                debugData.Type = type;
                debugData.Tiem = time;

                SedLogData(debugData);
            }
        }

        /// <summary>
        /// 保存日记到本地
        /// </summary>
        /// <param name="message">日记内容</param>
        private static void SaveErrorLog(string message)
        {
            string fileDif = Application.persistentDataPath + "/Debug/";

            if (!Directory.Exists(fileDif))
            {
                Directory.CreateDirectory(fileDif);
            }

            string filePath = fileDif + DateTime.Now.ToString("yyy-MM-dd") + ".txt";
            WriteData(filePath, message);
        }

        /// <summary>
        /// 写入数据到本地
        /// </summary>
        /// <param name="textPath">数据的路径</param>
        /// <param name="message">数据内容</param>
        private static  void WriteData(string textPath, string message)
        {
            if (string.IsNullOrEmpty(textPath) || string.IsNullOrEmpty(message)) return;

            StreamWriter sw = null;
            if (!File.Exists(textPath)) sw = File.CreateText(textPath);
            else sw = File.AppendText(textPath);

            sw.WriteLine(message + '\n');

            sw.Close();
            sw.Dispose();
        }

        /// <summary>初始化Soket（用来Dbug中传输Log）</summary>
        private void InitSocket()
        {
            iPEndPoint = new IPEndPoint(IPAddress.Broadcast, DebugDefine.UTP_PORT);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
        }

        /// <summary>
        /// 发送数据给调试端
        /// </summary>
        /// <param name="debugData">数据内容</param>
        private void SedLogData(DebugData debugData)
        {
            string sendStr = JsonUtility.ToJson(debugData);
            try
            {
                data = new byte[Encoding.UTF8.GetBytes(sendStr).Length];
                data = Encoding.UTF8.GetBytes(sendStr);
                socket.SendTo(data, data.Length, SocketFlags.None, iPEndPoint);
            }

            catch
            {
                Debug.LogWarning(GetType() + "/SedLogData() send debug errror!");
            }
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="bflag"></param>
        /// <returns></returns>
        private long GetTimeStamp(bool bflag = false)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long ret;
            if (bflag)
                ret = Convert.ToInt64(ts.TotalSeconds);
            else
                ret = Convert.ToInt64(ts.TotalMilliseconds);
            return ret;
        }

    }
}