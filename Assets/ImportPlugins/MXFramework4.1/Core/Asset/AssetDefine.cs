﻿/***
 * 
 *    Title: MXFramework
 *           主题: 资源模块全局定义
 *    Description: 
 *           功能：1.资源模块全局枚举定义
 *                2.资源模块全局委托定义
 *                3.资源模块全局数据定义            
 *                                  
 *    Date: 2020
 *    Version: v4.1版本
 *    Modify Recoder:      
 *
 */

using System;
using UnityEngine;

namespace Mx.Res
{
    public class AssetDefine 
    {
        /// <summary>Unity场景打包AB时候的后缀名称</summary>
        public const string AB_SCENE_EXTENSIONS = "u3d";
        /// <summary>Unity资源打包AB时候的后缀名称</summary>
        public const string AB_RES_EXTENSIONS = "data";
        /// <summary>压缩文件的后缀名称</summary>
        public const string UPK_EXTENSIONS = "upk";

        /// <summary>需要打包资源的文件路径</summary>
        public const string AB_RESOURCES = "Res/AbRes";

        /// <summary>得到Ab资源输入路径</summary>
        public static string GetABResourcePath()
        {
            return Application.dataPath + "/" + AB_RESOURCES;
        }

        /// <summary>获取AB资源加载路径</summary>
        public static string GetABLoadPath()
        {
            return Application.streamingAssetsPath + "/AssetsBundles" + "/" + GetPlatformName();
        }

        /// <summary>获取AB资源打包路径</summary>
        public static string GetBuildAssetOutPath()
        {
            return Application.streamingAssetsPath + "/AssetsBundles";
        }

        /// <summary>获取 Manifest 文件存放路径</summary>
        public static string GetManifestPath()
        {
            return GetBuildAssetOutPath() + "/" + GetPlatformName() + "/" + GetPlatformName();
            //return Paths.ModelsPath + "/" + GetPlatformName() + "/" + GetPlatformName();
        }

        /// <summary>获取平台名称</summary>
        public static string GetPlatformName()
        {
            string strReturnPlatformName = string.Empty;

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:

                    strReturnPlatformName = "Windows";
                    break;

                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:

                    strReturnPlatformName = "OSX";
                    break;

                case RuntimePlatform.IPhonePlayer:

                    strReturnPlatformName = "iOS";
                    break;

                case RuntimePlatform.Android:

                    strReturnPlatformName = "Android";
                    break;
            }

            return strReturnPlatformName;
        }

        /// <summary>Upk压缩文件输出目录</summary>
        public static string UpkOutPant = "/Users/chenyongliang/UnityProject/AssetsBundles/" + Application.productName;
        /// <summary>Upk压缩缓存路径</summary>
        public static string UpkTempCompressionPath = Application.persistentDataPath + "/Cache/Upk/Compression";
    }

}