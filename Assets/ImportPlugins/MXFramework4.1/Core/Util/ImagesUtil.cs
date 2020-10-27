using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Mx.Util
{
    /// <summary>图像工具类</summary>
    public class ImagesUtil
    {

        #region 图片转换成base64编码文本

        /// <summary>
        /// 图片转换成base64编码文本
        /// </summary>
        public static string ImgToBase64String(string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                string base64String = Convert.ToBase64String(buffer);
                return base64String;
            }
            catch (Exception e)
            {
                Debug.Log("ImgToBase64String 转换失败:" + e.Message);
                return null;
            }
        }

        #endregion


        #region bytes成base64编码文本

        /// <summary>
        /// 图片bytes[]转换成base64编码
        /// </summary>
        /// <param name="bytes">bytes</param>
        public static string ImgToBase64String(byte[] bytes)
        {
            try
            {
                string base64String = Convert.ToBase64String(bytes);
                return base64String;
            }
            catch (Exception e)
            {
                Debug.Log("ImgToBase64String 转换失败:" + e.Message);
                return null;
            }
        }

        #endregion

        public static Color32[] ResizeCanvas(Color32[] pixels, int oldWidth, int oldHeight, int width, int height)
        {
            var newPixels = new Color32[(width * height)];
            var wBorder = (width - oldWidth) / 2;
            var hBorder = (height - oldHeight) / 2;

            for (int r = 0; r < height; r++)
            {
                var oldR = r - hBorder;
                if (oldR < 0) { continue; }
                if (oldR >= oldHeight) { break; }

                for (int c = 0; c < width; c++)
                {
                    var oldC = c - wBorder;
                    if (oldC < 0) { continue; }
                    if (oldC >= oldWidth) { break; }

                    var oldI = oldR * oldWidth + oldC;
                    var i = r * width + c;
                    newPixels[i] = pixels[oldI];
                }
            }

            return newPixels;
        }


        #region 将Base64转换成为图片

        /// <summary>
        /// 将Base64转换成为图片
        /// </summary>
        /// <param name="base64String">Base64 string.</param>
        /// <param name="imgComponent">Image component.</param>
        public static void Base64ToImg(string base64String, Image imgComponent)
        {
            string base64 = base64String;
            byte[] bytes = Convert.FromBase64String(base64);
            Texture2D tex2D = new Texture2D(100, 100);
            tex2D.LoadImage(bytes);
            Sprite s = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f));
            imgComponent.sprite = s;
            Resources.UnloadUnusedAssets();
        }

        /// <summary>将Base64转换成为Bytes</summary>
        public static byte[] Base64ToBytes(string base64String)
        {
            string base64 = base64String;
            byte[] bytes = Convert.FromBase64String(base64);
            return bytes;
        }

        #endregion


        #region 图片裁剪

        public static Texture2D ScaleTextureCutOut(Texture2D originalTexture, float startX, float startY, float originalWidth, float originalHeight)
        {
            originalWidth = Mathf.Clamp(originalWidth, 0, Mathf.Max(originalTexture.width - startX, 0));
            originalHeight = Mathf.Clamp(originalHeight, 0, Mathf.Max(originalTexture.height - startY, 0));
            Texture2D newTexture = new Texture2D(Mathf.CeilToInt(originalWidth), Mathf.CeilToInt(originalHeight));
            int maxX = originalTexture.width - 1;
            int maxY = originalTexture.height - 1;
            for (int y = 0; y < newTexture.height; y++)
            {
                for (int x = 0; x < newTexture.width; x++)
                {
                    float targetX = x + startX;
                    float targetY = y + startY;
                    int x1 = Mathf.Min(maxX, Mathf.FloorToInt(targetX));
                    int y1 = Mathf.Min(maxY, Mathf.FloorToInt(targetY));
                    int x2 = Mathf.Min(maxX, x1 + 1);
                    int y2 = Mathf.Min(maxY, y1 + 1);

                    float u = targetX - x1;
                    float v = targetY - y1;
                    float w1 = (1 - u) * (1 - v);
                    float w2 = u * (1 - v);
                    float w3 = (1 - u) * v;
                    float w4 = u * v;
                    Color color1 = originalTexture.GetPixel(x1, y1);
                    Color color2 = originalTexture.GetPixel(x2, y1);
                    Color color3 = originalTexture.GetPixel(x1, y2);
                    Color color4 = originalTexture.GetPixel(x2, y2);
                    Color color = new Color(Mathf.Clamp01(color1.r * w1 + color2.r * w2 + color3.r * w3 + color4.r * w4),
                                            Mathf.Clamp01(color1.g * w1 + color2.g * w2 + color3.g * w3 + color4.g * w4),
                                            Mathf.Clamp01(color1.b * w1 + color2.b * w2 + color3.b * w3 + color4.b * w4),
                                            Mathf.Clamp01(color1.a * w1 + color2.a * w2 + color3.a * w3 + color4.a * w4)
                                            );
                    newTexture.SetPixel(x, y, color);
                }
            }
            newTexture.anisoLevel = 2;
            newTexture.Apply();
            return newTexture;
        }

        /// <summary>
        /// 以IO方式进行加载
        /// </summary>
        public static Texture2D LoadImageByIo(string url, int width = 800, int height = 400)
        {
            //创建文件读取流
            FileStream fileStream = new FileStream(url, FileMode.Open, FileAccess.Read);
            //创建文件长度缓冲区
            byte[] bytes = new byte[fileStream.Length];
            //读取文件
            fileStream.Read(bytes, 0, (int)fileStream.Length);

            //释放文件读取流
            fileStream.Close();
            //释放本机屏幕资源
            fileStream.Dispose();
            fileStream = null;

            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(bytes);

            return texture;
        }

        #endregion

    }
}
