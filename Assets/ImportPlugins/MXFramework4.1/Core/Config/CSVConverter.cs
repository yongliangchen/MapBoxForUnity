using System;
using UnityEngine;

namespace Mx.Config
{

    /// <summary>CSV转换工具</summary>
    public class CSVConverter
    {
        public static string[] SerializeCSVParameter(TextAsset csvData)
        {
            string[] lineArray = csvData.text.Replace("\n", string.Empty).Split("\r"[0]);
            return lineArray[0].Split(',');
        }

        /// <summary>
        /// 序列化CSV格式数据
        /// </summary>
        /// <param name="csvData">CSV数据</param>
        /// <returns></returns>
        public static string[][] SerializeCSVData(string csvData)
        {
            string[][] csv;
            string[] lineArray = csvData.Replace("\n", string.Empty).Split("\r"[0]);
            csv = new string[lineArray.Length - 1][];
            for (int i = 0; i < lineArray.Length - 1; i++)
            {
                csv[i] = lineArray[i + 1].Split(',');
            }

            return csv;
        }

        /// <summary>
        /// 序列化CSV格式数据(方法重载)
        /// </summary>
        /// <param name="csvData">CSV数据</param>
        /// <returns></returns>
        public static string[][] SerializeCSVData(TextAsset csvData)
        {
           return SerializeCSVData(csvData.text);
        }

        public static T[] ConvertToArray<T>(string value)
        {
            string[] temp = value.Split(';');
            int arrayLength = 0;

            for (int cnt = 0; cnt < temp.Length; cnt++)
            {
                if (string.IsNullOrEmpty(temp[cnt]))
                {
                    continue;
                }
                arrayLength++;
            }

            T[] array = new T[arrayLength];
            int pointer = 0;
            for (int cnt = 0; cnt < temp.Length; cnt++)
            {
                if (string.IsNullOrEmpty(temp[cnt]))
                    continue;
                array[pointer] = (T)Convert.ChangeType(temp[cnt], typeof(T));
                pointer++;
            }

            return array;

        }

    }

}
