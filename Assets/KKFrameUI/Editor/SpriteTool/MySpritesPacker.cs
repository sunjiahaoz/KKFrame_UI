#if UNITY_EDITOR
using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
namespace KK.Frame.Util.SpritePacker
{

    public class MySpritesPacker
    {
        //   [MenuItem("Tools/SpritesPacker/TexturePacker")]
        public static void BuildTexturePacker(string _path, string name)
        {
            string inputPath = CompareAssetPath(_path);
            string[] imagePath = Directory.GetFiles(inputPath);
            foreach (string path in imagePath)
            {
                if (!path.Contains(name))
                    continue;

                if (Path.GetExtension(path) == ".png" || Path.GetExtension(path) == ".PNG")
                {
                    string sheetPath = GetAssetPath(path);
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(sheetPath);
                    Debug.Log(texture.name);

                    AssetDatabase.Refresh();
                    FileStream fs = new FileStream(inputPath + texture.name + ".txml", FileMode.Open);
                    StreamReader sr = new StreamReader(fs);
                    string jText = sr.ReadToEnd();
                    fs.Close();
                    sr.Close();
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(jText);
                    XmlNodeList elemList = xml.GetElementsByTagName("SubTexture");

                    Dictionary<string, Vector4> tIpterMap = new Dictionary<string, Vector4>();
                    WriteMeta(elemList, texture, tIpterMap);
                }
                //删除配置文件
                if (File.Exists(path) && Path.GetExtension(path) == ".txml")
                {
                    File.Delete(path);
                }
            }
            AssetDatabase.Refresh();
        }
        //如果这张图集已经拉好了9宫格，需要先保存起来
        static void SaveBoreder(Dictionary<string, Vector4> tIpterMap, TextureImporter tIpter)
        {
            for (int i = 0, size = tIpter.spritesheet.Length; i < size; i++)
            {
                tIpterMap.Add(tIpter.spritesheet[i].name, tIpter.spritesheet[i].border);
            }
        }

        static TextureImporter GetTextureIpter(Texture2D texture)
        {
            TextureImporter textureIpter = null;
            string impPath = AssetDatabase.GetAssetPath(texture);
            textureIpter = TextureImporter.GetAtPath(impPath) as TextureImporter;
            return textureIpter;
        }

        static TextureImporter GetTextureIpter(string path)
        {
            TextureImporter textureIpter = null;
            Texture2D textureOrg = AssetDatabase.LoadAssetAtPath<Texture2D>(GetAssetPath(path));
            string impPath = AssetDatabase.GetAssetPath(textureOrg);
            textureIpter = TextureImporter.GetAtPath(impPath) as TextureImporter;
            return textureIpter;
        }
        //写信息到SpritesSheet里
        static void WriteMeta(XmlNodeList elemList, Texture2D texture, Dictionary<string, Vector4> borders)
        {
            // string path = string.Format("Assets/Packer/{0}/{1}.png", sheetName, sheetName);
            // string path = GetAssetPath(sheetName);
            // Texture2D texture =  AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            string impPath = AssetDatabase.GetAssetPath(texture);
            TextureImporter asetImp = TextureImporter.GetAtPath(impPath) as TextureImporter;
            asetImp.textureType = TextureImporterType.Advanced;
            SpriteMetaData[] metaData = new SpriteMetaData[elemList.Count];
            for (int i = 0, size = elemList.Count; i < size; i++)
            {
                XmlElement node = (XmlElement)elemList.Item(i);
                Rect rect = new Rect();
                rect.x = int.Parse(node.GetAttribute("x"));
                rect.y = texture.height - int.Parse(node.GetAttribute("y")) - int.Parse(node.GetAttribute("height"));
                rect.width = int.Parse(node.GetAttribute("width"));
                rect.height = int.Parse(node.GetAttribute("height"));
                metaData[i].rect = rect;
                metaData[i].pivot = new Vector2(0.5f, 0.5f);
                metaData[i].name = node.GetAttribute("name");
                if (borders.ContainsKey(metaData[i].name))
                {
                    metaData[i].border = borders[metaData[i].name];
                }
            }
            asetImp.spritesheet = metaData;
            asetImp.textureType = TextureImporterType.Sprite;
            asetImp.spriteImportMode = SpriteImportMode.Multiple;
            asetImp.mipmapEnabled = false;
            asetImp.textureFormat = TextureImporterFormat.AutomaticTruecolor;

            //asetImp.textureType = TextureImporterType.Advanced;
            //asetImp.textureFormat = TextureImporterFormat.ETC_RGB4;
            //asetImp.compressionQuality = 100;
            //asetImp.spriteImportMode = SpriteImportMode.Multiple;
            //asetImp.mipmapEnabled = false;
            asetImp.SaveAndReimport();
        }

        static string GetAssetPath(string path)
        {
            string[] seperator = { "Assets" };
            string p = "Assets" + path.Split(seperator, StringSplitOptions.RemoveEmptyEntries)[1];
            return p;
        }
        static string CompareAssetPath(string path)
        {
            string[] seperator = { "Assets" };
            string[] temp = path.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            string p = Application.dataPath + temp[0];
            return p;
        }

    }

    internal class TextureIpter
    {
        public string spriteName = "";
        public Vector4 border = new Vector4();
        public TextureIpter() { }
        public TextureIpter(string spriteName, Vector4 border)
        {
            this.spriteName = spriteName;
            this.border = border;
        }
    }
}
#endif