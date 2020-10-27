using System.IO;
using UnityEngine;
using System;

namespace Mx.Util
{
    /// <summary>拷贝文件</summary>
    public class CopyFiles
    {
        /// <summary>
        /// 拷贝文件到指定路径(需要加文件后缀)
        /// </summary>
        /// <param name="pStrFilePath">需要拷贝文件的路径</param>
        /// <param name="pPerFilePath">拷贝到路径</param>
        /// <param name="finish">结束回调</param>
        public static void Copy(string pStrFilePath, string pPerFilePath, Action<string> finish = null)
        {
            if (string.IsNullOrEmpty(pStrFilePath) || string.IsNullOrEmpty(pPerFilePath))
            {
                Debug.LogWarning("CopyFiles/Copy/" + "copy file wrong! file path is null!");
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                pStrFilePath = @"file://" + pStrFilePath;
            }

            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                pStrFilePath = @"file:///" + pStrFilePath;
            }

            string[] tempPerFilePathArr = pPerFilePath.Split('/');
            string tempPerFilePath = pPerFilePath.Replace(tempPerFilePathArr[tempPerFilePathArr.Length - 1], null);
            if (!Directory.Exists(tempPerFilePath)) Directory.CreateDirectory(tempPerFilePath);

            WWW ww = new WWW(pStrFilePath);

            while (!ww.isDone) { }
            if (string.IsNullOrEmpty(ww.error))
            {
                var buffer = ww.bytes;
                if (File.Exists(pPerFilePath))
                    File.Delete(pPerFilePath);
                var ws = File.Create(pPerFilePath);
                ws.Write(buffer, 0, buffer.Length);
                ws.Close();

                if (finish != null) finish(null);

                Debug.Log("CopyFiles/Copy/" + "copy file success:" + pPerFilePath);
            }
            else
            {
                if (finish != null) finish(ww.error);

                Debug.LogWarning("CopyFiles/Copy/" + "copy file wrong !!!!   " + ww.error);
            }
            ww.Dispose();
        }
    }
}