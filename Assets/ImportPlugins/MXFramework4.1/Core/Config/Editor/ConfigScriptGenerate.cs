using UnityEngine;
using UnityEditor;
using System.IO;
using Mx.Util;

namespace Mx.Config
{
    /// <summary>自动生成配置表</summary>
    public class ConfigScriptGenerate
    {
        private static int DATA_ID;

        private static string REGISTER_LIST;

        private static string CONVERT_LIST;

        /// <summary>自动生成脚本</summary>
        [MenuItem("MXFramework/Config/Generate Script", false, 101)]
        public static void GenerateScript()
        {
            Initialize();
            CreateIDatabaseScript();
            CreateDatabaseScript();
            CreateDatabaseManagerScript();

            AssetDatabase.Refresh();
        }

        /// <summary>初始化</summary>
        private static void Initialize()
        {
            DATA_ID = 0;
            REGISTER_LIST = string.Empty;
            CONVERT_LIST = string.Empty;

            if (Directory.Exists(ConfigDefine.GENERATE_SCRIPT_PATH))Directory.Delete(ConfigDefine.GENERATE_SCRIPT_PATH, true);
            Directory.CreateDirectory(ConfigDefine.GENERATE_SCRIPT_PATH);

            if (Directory.Exists(ConfigDefine.CSV_STREAMING_ENCRYPT_PATH)) Directory.Delete(ConfigDefine.CSV_STREAMING_ENCRYPT_PATH, true);
            Directory.CreateDirectory(ConfigDefine.CSV_STREAMING_ENCRYPT_PATH);

            if (Directory.Exists(ConfigDefine.CSV_RESOUCES_ENCRYPT_PATH)) Directory.Delete(ConfigDefine.CSV_RESOUCES_ENCRYPT_PATH, true);
            Directory.CreateDirectory(ConfigDefine.CSV_RESOUCES_ENCRYPT_PATH);
        }

        /// <summary>创建Base脚本</summary>
        private static void CreateIDatabaseScript()
        {
            string template = GetTemplate(ConfigDefine.TEMPLATE_IDATABASE_PATH);
            GenerateScript("IDatabase", template);
        }

        /// <summary>
		/// 获取数据模板
		/// </summary>
		/// <param name="path">模板路径</param>
		/// <returns></returns>
        private static string GetTemplate(string path)
        {
            //TextAsset txt = (TextAsset)AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
            TextAsset txt = Resources.Load<TextAsset>(path);
            return txt.text;
        }

        /// <summary>
		/// 生成脚本
		/// </summary>
		/// <param name="dataName">数据名称</param>
		/// <param name="data">数据</param>
        private static void GenerateScript(string dataName, string data)
        {
            dataName = ConfigDefine.GENERATE_SCRIPT_PATH + dataName + ".cs";

            if (File.Exists(dataName)) File.Delete(dataName);

            StreamWriter sr = File.CreateText(dataName);
            sr.WriteLine(data);
            sr.Close();
        }

        /// <summary>创建数据基类脚本</summary>
        private static void CreateDatabaseScript()
        {
            string[] csvStreamingPaths = Directory.GetFiles(ConfigDefine.CsvStreamingPath, "*.csv", SearchOption.AllDirectories);
            string[] csvResourcesPaths = Directory.GetFiles(ConfigDefine.CsvResourcesPath, "*.csv", SearchOption.AllDirectories);

            CreateDatabaseScript(EnumCofigLoadType.Streaming, csvStreamingPaths);
            CreateDatabaseScript(EnumCofigLoadType.Resources, csvResourcesPaths);
        }

		/// <summary>
		/// 创建数据基类脚本
		/// </summary>
		/// <param name="cofigLoadType">配置表加载类型</param>
		/// <param name="csvPaths">CSV数据路径</param>
		private static void CreateDatabaseScript(EnumCofigLoadType cofigLoadType, string[] csvPaths)
        {
            string assetPath = "";

            TextAsset textAsset = null;

            for (int cnt = 0; cnt < csvPaths.Length; cnt++)
            {
                assetPath = "Assets" + csvPaths[cnt].Replace(Application.dataPath, "").Replace('\\', '/');

                textAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset));

                REGISTER_LIST += string.Format("RegisterDataType(new {0}Database());\n", textAsset.name);

                if (cnt != csvPaths.Length - 1)
                    REGISTER_LIST += "\t\t\t";
                CONVERT_LIST += string.Format("CsvToJsonConverter.Convert<{0}Data>(\"{0}\"); \n", textAsset.name);

                if (cnt != csvPaths.Length - 1)
                    CONVERT_LIST += "\t\t\t";

                CreateDatabaseScript(cofigLoadType,textAsset);
            }
        }

        /// <summary>
		/// 创建数据基类脚本（方法重载）
		/// </summary>
		/// <param name="cofigLoadType">配置表类型</param>
		/// <param name="textAsset">text格式数据</param>
        private static void CreateDatabaseScript(EnumCofigLoadType cofigLoadType, TextAsset textAsset)
        {
            DATA_ID++;

            string template = string.Empty;
            string configPath = string.Empty;

            switch (cofigLoadType)
            {
                case EnumCofigLoadType.Streaming:

                    template = GetTemplate(ConfigDefine.TEMPLATE_DATABASE_STREAMING_PATH);
                    configPath = ConfigDefine.CSV_STREAMING_ENCRYPT_PATH + textAsset.name + ".txt";
                    break;

                case EnumCofigLoadType.Resources:

                    template = GetTemplate(ConfigDefine.TEMPLATE_DATABASE_RESOURECES_PATH);
                    configPath = ConfigDefine.CSV_RESOUCES_ENCRYPT_PATH + textAsset.name + ".txt";
                    break;
            }
    
            template = template.Replace("$DataClassName", textAsset.name + "Data");
            template = template.Replace("$DataAttributes", GetClassParameters(textAsset));
            template = template.Replace("$CsvSerialize", GetCsvSerialize(textAsset));
            template = template.Replace("$DataTypeName", textAsset.name + "Database");
            template = template.Replace("$DataID", DATA_ID.ToString());
            template = template.Replace("$DataPath", "\"" + textAsset.name + "\"");

            GenerateScript(textAsset.name + "Database", template);
            CsvEncrypt(configPath, textAsset.text);
        }

        /// <summary>
		/// 获取自动生成类的参数
		/// </summary>
		/// <param name="textAsset">text数据</param>
		/// <returns></returns>
        private static string GetClassParameters(TextAsset textAsset)
        {
            string[] csvParameter = CSVConverter.SerializeCSVParameter(textAsset);
            int keyCount = csvParameter.Length;

            string classParameters = string.Empty;

            for (int cnt = 0; cnt < keyCount; cnt++)
            {
                string[] attributes = csvParameter[cnt].Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);
                classParameters += string.Format("public {0} {1};", attributes[0], attributes[1]);

                if (cnt != keyCount - 1)
                {
                    classParameters += "\n";
                    classParameters += "\t\t";
                }
            }

            return classParameters;
        }

		/// <summary>
		/// 获取CSV的序列化数据
		/// </summary>
		/// <param name="textAsset">text数据</param>
		/// <returns></returns>
		private static string GetCsvSerialize(TextAsset textAsset)
        {
            string[] csvParameter = CSVConverter.SerializeCSVParameter(textAsset);

            int keyCount = csvParameter.Length;

            string csvSerialize = string.Empty;

            for (int cnt = 0; cnt < keyCount; cnt++)
            {
                string[] attributes = csvParameter[cnt].Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);

                if (attributes[0] == "string")
                {
                    csvSerialize += string.Format("m_tempData.{0} = m_datas[cnt][{1}];", attributes[1], cnt);
                }
                else if (attributes[0] == "bool")
                {
                    csvSerialize += GetCsvSerialize(attributes, cnt, "0");
                }
                else if (attributes[0] == "int")
                {
                    csvSerialize += GetCsvSerialize(attributes, cnt, "0");
                }
                else if (attributes[0] == "float")
                {
                    csvSerialize += GetCsvSerialize(attributes, cnt, "0.0f");
                }
                else if (attributes[0] == "string[]")
                {
                    csvSerialize += string.Format("m_tempData.{0} = CSVConverter.ConvertToArray<string>(m_datas[cnt][{1}]);",
                        attributes[1], cnt);
                }
                else if (attributes[0] == "bool[]")
                {
                    csvSerialize += string.Format("m_tempData.{0} = CSVConverter.ConvertToArray<bool>(m_datas[cnt][{1}]);",
                        attributes[1], cnt);
                }
                else if (attributes[0] == "int[]")
                {
                    csvSerialize += string.Format("m_tempData.{0} = CSVConverter.ConvertToArray<int>(m_datas[cnt][{1}]);",
                        attributes[1], cnt);
                }
                else if (attributes[0] == "float[]")
                {
                    csvSerialize += string.Format("m_tempData.{0} = CSVConverter.ConvertToArray<float>(m_datas[cnt][{1}]);",
                        attributes[1], cnt);
                }

                if (cnt != keyCount - 1)
                {
                    csvSerialize += "\n";
                    csvSerialize += "\t\t";
                }
            }

            return csvSerialize;
        }

		/// <summary>
		/// 获取CSV的序列化数据
		/// </summary>
		/// <param name="attributes">属性</param>
		/// <param name="arrayCount">数组数</param>
		/// <param name="defaultValue">默认值</param>
		/// <returns></returns>
		private static string GetCsvSerialize(string[] attributes, int arrayCount, string defaultValue)
        {
            string csvSerialize = "";
            csvSerialize += string.Format("\n\t\t\tif(!{0}.TryParse(m_datas[cnt][{1}], out m_tempData.{2}))\n", attributes[0], arrayCount, attributes[1]);

            csvSerialize += "\t\t\t{\n";
            csvSerialize += string.Format("\t\t\t\tm_tempData.{0} = {1};\n", attributes[1], defaultValue);
            csvSerialize += "\t\t\t}\n";

            return csvSerialize;
        }

        /// <summary>
		/// CSV数据加密
		/// </summary>
		/// <param name="textPath">Text路径</param>
		/// <param name="textAsset">Text数据</param>
        private static void CsvEncrypt(string textPath, string textAsset)
        {
            string data = StringEncrypt.EncryptDES(textAsset);

            if (File.Exists(textPath)) File.Delete(textPath);

            StreamWriter sr = File.CreateText(textPath);
            sr.WriteLine(data);
            sr.Close();
        }

        /// <summary>创建数据基类管理类</summary>
        private static void CreateDatabaseManagerScript()
        {
            string template = GetTemplate(ConfigDefine.TEMPLATE_DATABASEMANAGER_PATH);
            template = template.Replace("$RegisterList", REGISTER_LIST);
            GenerateScript("DatabaseManager", template);
        }

    }

}