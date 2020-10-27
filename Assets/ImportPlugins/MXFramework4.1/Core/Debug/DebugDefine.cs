using UnityEngine;
using System;

namespace Mx.Log
{
    /// <summary>Debug模块的一些公共数据定义类</summary>
    public sealed class DebugDefine
    {
        /// <summary>UTP端口号</summary>
        public const int UTP_PORT = 9621;

        /// <summary>是否是调试模式</summary>
        public static bool IsDebugMode
        {
            get
            {
                #if DEBUG_MODE
                bool debugMode = true;
                #else
                bool debugMode = false;
                #endif
                return debugMode;
            }
        }
    }

    [Serializable]
    public class DebugData
    {
        public string ID;
        public int Count = 1;
        public string Condition;
        public string StackTrace;
        public LogType Type;
        public string Tiem;
    }
}