#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Reflection;

namespace KK.Frame.Util.SpritePacker
{
    public enum SeperateType
    {
        normal,
        SeperateAlpha,
    }
    /// <summary>
    /// 显示的控制类
    /// </summary>
    public class SeperateSpriteToolController
    {
        [MenuItem("Assets/SpritesTool/SeperateSprite/1.1 分离Alpha通道 （ 贴图压缩格式为TureColor 再点击此项）")]
        static void SeperateAllTexturesRGBandAlphaChannel()
        {
            SegmentationTexture.SeperateAllTexturesRGBandAlphaChannel();
        }
        [MenuItem("Assets/SpritesTool/SeperateSprite/1.2 创建材质球")]
        static void MatrialGenerator()
        {
            SegmentationTexture.MatrialGenerator();
        }

        [MenuItem("Assets/SpritesTool/SeperateSprite/2.1 分割sprite  (分割png 有xml文件)")]
        static void SeperateSprite()
        {
            SegmentationTexture.SeperateAllTexturesRGBandAlphaChannel(SeperateType.normal);
        }
    }

    public class SegmentationTexture
    {

        private static string defaultWhiteTexPath_relative = "Assets/TexturePacker/123/rgbaText.png";
        private static Texture2D defaultWhiteTex = null;
        public static void SeperateAllTexturesRGBandAlphaChannel(SeperateType type = SeperateType.SeperateAlpha)
        {
            Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            if (os.Length != 1)
            {
                return;
            }
            string filePath = AssetDatabase.GetAssetPath(os[0].GetInstanceID());

            if (!string.IsNullOrEmpty(filePath) && IsTextureFile(filePath) && !IsTextureConverted(filePath))   //full name    
            {
                if (type == SeperateType.SeperateAlpha)
                    SeperateRGBAandlphaChannel(filePath);
                else if (type == SeperateType.normal)
                    SegmentationTextureToSprite(filePath);
            }

            AssetDatabase.Refresh();    //Refresh to ensure new generated RBA and Alpha textures shown in Unity as well as the meta file  
            Debug.Log("Finish Departing.");
        }
        #region process texture
        public static float sizeScale = 0.5f;

        static void SeperateRGBAandlphaChannel(string _texPath)
        {
            // string assetRelativePath = GetRelativeAssetPath(_texPath);
            string assetRelativePath = _texPath;
            SetTextureReadableEx(assetRelativePath);    //set readable flag and set textureFormat TrueColor  

            TextureImporter ti = null;
            try
            {
                ti = (TextureImporter)TextureImporter.GetAtPath(assetRelativePath);
                ti.textureType = TextureImporterType.Advanced;
                ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                ti.SaveAndReimport();
            }
            catch
            {
                Debug.LogError("Load Texture failed: " + assetRelativePath);
                return;
            }
            if (ti == null)
            {
                return;
            }

            Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D;  //not just the textures under Resources file    
            if (!sourcetex)
            {
                Debug.LogError("Load Texture Failed : " + assetRelativePath);
                return;
            }
            bool bGenerateMipMap = ti.mipmapEnabled;    //same with the texture import setting        

            Texture2D rgbTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, bGenerateMipMap);
            rgbTex.SetPixels(sourcetex.GetPixels());
            rgbTex.Apply();
            Texture2D mipMapTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGBA32, true);  //Alpha Channel needed here  
            mipMapTex.SetPixels(sourcetex.GetPixels());
            mipMapTex.Apply();
            Color[] colors2rdLevel = mipMapTex.GetPixels(0);   //Second level of Mipmap  
            Color[] colorsAlpha = new Color[colors2rdLevel.Length];
            //         if (colors2rdLevel.Length != (mipMapTex.width + 1) / 2 * (mipMapTex.height + 1) / 2)
            //         {
            //             Debug.LogError("Size Error.");
            //             return;
            //         }
            bool bAlphaExist = false;
            for (int i = 0; i < colors2rdLevel.Length; ++i)
            {
                colorsAlpha[i].r = colors2rdLevel[i].a;
                colorsAlpha[i].g = colors2rdLevel[i].a;
                colorsAlpha[i].b = colors2rdLevel[i].a;

                if (!Mathf.Approximately(colors2rdLevel[i].a, 1.0f))
                {
                    bAlphaExist = true;
                }
            }
            Texture2D alphaTex = null;
            if (bAlphaExist)
            {
                //alphaTex = new Texture2D((sourcetex.width + 1) / 2, (sourcetex.height + 1) / 2, TextureFormat.RGB24, bGenerateMipMap);
                alphaTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, bGenerateMipMap);
            }
            else
            {
                alphaTex = new Texture2D(defaultWhiteTex.width, defaultWhiteTex.height, TextureFormat.RGB24, false);
            }

            alphaTex.SetPixels(colorsAlpha);

            rgbTex.Apply();
            alphaTex.Apply();

            byte[] bytes = rgbTex.EncodeToPNG();
            File.WriteAllBytes(assetRelativePath, bytes);
            byte[] alphabytes = alphaTex.EncodeToPNG();
            string alphaTexRelativePath = GetAlphaTexPath(_texPath);
            File.WriteAllBytes(alphaTexRelativePath, alphabytes);

            ReImportAsset(assetRelativePath, rgbTex.width, rgbTex.height);
            ReImportAsset(alphaTexRelativePath, alphaTex.width, alphaTex.height);
            Debug.Log("Succeed Departing : " + assetRelativePath);

            #region _图集分离_
            SegmentationTextureToSprite(assetRelativePath);
            AssetDatabase.Refresh();
            #endregion

//             #region _创建材质球_
//             string materialPath = GetMaterialTexPath(assetRelativePath);
//             Debug.Log(materialPath);
//          //   Shader shader = Shader.Find("Custom/UIETC");
//             Material material = new Material(Shader.Find("Custom/UIETC"));
//             AssetDatabase.CreateAsset(material, materialPath);
//             Debug.Log(material.HasProperty("_MainTex") + "  " + rgbTex);
//             material.SetTexture("_MainTex", rgbTex);
//             material.SetTexture("_AlphaTex", alphaTex);
//             AssetDatabase.ImportAsset(materialPath);
//             AssetDatabase.Refresh();
//             #endregion

        }
        //创建材质球
        public static void MatrialGenerator()
        {
            //获取路径
            Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            if (os.Length != 1)
            {
                return;
            }
            string filePath = AssetDatabase.GetAssetPath(os[0].GetInstanceID());
            if(System.IO.Path.GetFileNameWithoutExtension(filePath).Contains("_Alpha"))
            {
                Debug.LogError("不要选择Alpha通道贴图");
                return;
            }
            string assetRelativePath = filePath;
            //获取贴图
            Texture2D alphaTex = AssetDatabase.LoadAssetAtPath(GetAlphaTexPath(assetRelativePath), typeof(Texture2D)) as Texture2D;
            Texture2D rgbTex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D;
            
            string materialPath = GetMaterialTexPath(assetRelativePath);
            Debug.Log(materialPath);
         //   Shader shader = Shader.Find("Custom/UIETC");
            Material material = new Material(Shader.Find("Custom/UIETC"));
            AssetDatabase.CreateAsset(material, materialPath);
            Debug.Log(material.HasProperty("_MainTex") + "  " + rgbTex);
            material.SetTexture("_MainTex", rgbTex);
            material.SetTexture("_AlphaTex", alphaTex);
            AssetDatabase.ImportAsset(materialPath);
            AssetDatabase.Refresh();
        }
        public static void SegmentationTextureToSprite_ByXmlPath(string xmlPath)
        {
            if (File.Exists(xmlPath))
            {
                string[] str = xmlPath.Split('/');
                xmlPath = "";
                for (int j = 0, count = str.Length; j < count - 1; j++)
                    xmlPath += string.Format("{0}/", str[j]);

                MySpritesPacker.BuildTexturePacker(xmlPath,
                    str[str.Length - 1].Split(new string[] { "txml" }, System.StringSplitOptions.RemoveEmptyEntries)[0]);
            }
            else
            {
                Debug.Log(" no txml : " + xmlPath);
                return;
            }
        }
        public static void SegmentationTextureToSprite(string assetRelativePath)
        {
            string xmlPath = GetXMLFile(assetRelativePath);
            SegmentationTextureToSprite_ByXmlPath(xmlPath);           
        }

        static void ReImportAsset(string path, int width, int height)
        {
            try
            {
                AssetDatabase.ImportAsset(path);
            }
            catch
            {
                Debug.LogError("Import Texture failed: " + path);
                return;
            }

            TextureImporter importer = null;
            try
            {
                importer = (TextureImporter)TextureImporter.GetAtPath(path);
            }
            catch
            {
                Debug.LogError("Load Texture failed: " + path);
                return;
            }
            if (importer == null)
            {
                return;
            }
            importer.maxTextureSize = Mathf.Max(width, height);
            importer.anisoLevel = 0;
            importer.isReadable = false;  //increase memory cost if readable is true 
            importer.textureType = TextureImporterType.Advanced;
            importer.textureFormat = TextureImporterFormat.ETC_RGB4;
            importer.mipmapEnabled = false;
            if (path.Contains("_Alpha"))
            {
                importer.textureType = TextureImporterType.Image;
                importer.textureFormat = TextureImporterFormat.AutomaticCompressed;
            }
            importer.compressionQuality = 100;
            AssetDatabase.ImportAsset(path);
        }


        static void SetTextureReadableEx(string _relativeAssetPath)    //set readable flag and set textureFormat TrueColor  
        {
            TextureImporter ti = null;
            try
            {
                ti = (TextureImporter)TextureImporter.GetAtPath(_relativeAssetPath);
            }
            catch
            {
                Debug.LogError("Load Texture failed: " + _relativeAssetPath);
                return;
            }
            if (ti == null)
            {
                return;
            }
            ti.isReadable = true;
            ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;      //this is essential for departing Textures for ETC1. No compression format for following operation.  
            ti.textureType = TextureImporterType.Advanced;
            ti.mipmapEnabled = false;
            AssetDatabase.ImportAsset(_relativeAssetPath);
        }

        static bool GetDefaultWhiteTexture()
        {
            defaultWhiteTex = AssetDatabase.LoadAssetAtPath(defaultWhiteTexPath_relative, typeof(Texture2D)) as Texture2D;  //not just the textures under Resources file    
            if (!defaultWhiteTex)
            {
                Debug.LogError("Load Texture Failed : " + defaultWhiteTexPath_relative);
                return false;
            }
            return true;
        }

        #endregion

        #region string or path helper

        static bool IsTextureFile(string _path)
        {
            string path = _path.ToLower();
            return path.EndsWith(".psd") || path.EndsWith(".tga") || path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".bmp") || path.EndsWith(".tif") || path.EndsWith(".gif");
        }

        static bool IsTextureConverted(string _path)
        {
            return _path.Contains("_RGB.") || _path.Contains("_Alpha.");
        }

        static string GetRGBTexPath(string _texPath)
        {
            return GetTexPath(_texPath, "_RGB.", "png");
        }

        static string GetAlphaTexPath(string _texPath)
        {
            return GetTexPath(_texPath, "_Alpha.", "png");
        }
        static string GetMaterialTexPath(string _texPath)
        {
            return GetTexPath(_texPath, "_Material.", "mat");
        }

        static string GetTexPath(string _texPath, string _texRole, string _suffix)
        {
            string dir = System.IO.Path.GetDirectoryName(_texPath);
            string filename = System.IO.Path.GetFileNameWithoutExtension(_texPath);
            string result = dir + "/" + filename + _texRole + _suffix;
            return result;
        }

        static string GetRelativeAssetPath(string _fullPath)
        {
            _fullPath = GetRightFormatPath(_fullPath);
            int idx = _fullPath.IndexOf("Assets");
            string assetRelativePath = _fullPath.Substring(idx);
            return assetRelativePath;
        }

        static string GetRightFormatPath(string _path)
        {
            return _path.Replace("\\", "/");
        }
        static string GetXMLFile(string _path)
        {

            return _path.Replace(".png", ".txml");
        }

        #endregion
    }
}

#endif