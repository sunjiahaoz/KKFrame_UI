using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace KK.Frame.Util.SpritePacker
{

    public class SpriteToolConfigEditor : EditorWindow
    {

        SpriteToolConfigData _config = null;
        SpriteToolConfigData config
        {
            get
            {
                if (_config == null)
                {
                    _config = AssetDatabase.LoadAssetAtPath<SpriteToolConfigData>(SpriteToolConfigData._strConfigAssetPath);
                    if (_config == null)
                    {
                        _config = ScriptableObject.CreateInstance<SpriteToolConfigData>();
                        _config._strTPExePath = "请选择TexturePacker.exe的路径";
                        AssetDatabase.CreateAsset(_config, SpriteToolConfigData._strConfigAssetPath);
                        EditorUtility.SetDirty(_config);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
                return _config;
            }
        }

        #region _Config_
        GUIStyle _style = new GUIStyle();

        [MenuItem("Assets/SpritesTool/配置")]
        public static void AddWindow()
        {
            SpriteToolConfigEditor window = (SpriteToolConfigEditor)EditorWindow.GetWindow<SpriteToolConfigEditor>("SpriteTool配置");
            window.Init();
            window.Show();
        }

        public void Init()
        {
            _style.fontSize = 12;
            _style.richText = true;
        }
        void Show_Config(SpriteToolConfigData config)
        {

            {
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                // 路径
                GUILayout.Label("TexturePacker 路径");
                GUILayout.BeginHorizontal();
                if (!File.Exists(config._strTPExePath))
                {
                    GUILayout.Label("<color=red>请重新选择TexturePacker.exe的路径</color>", _style);
                }
                else
                {
                    GUILayout.Label("<color=green>" + config._strTPExePath + "</color>", _style);
                }
                if (GUILayout.Button("浏览...", GUILayout.MinWidth(20)))
                {
                    config._strTPExePath = EditorUtility.OpenFilePanel("选择TexturePacker.exe", "", "exe");
                    Debug.Log("<color=green>[log]</color>---" + config._strTPExePath);
                }
                GUILayout.EndHorizontal();

                //GUILayout.BeginHorizontal();
                _config._nMaxWidth = EditorGUILayout.IntField("最大尺寸_宽:", _config._nMaxWidth);
                _config._nMaxHeight = EditorGUILayout.IntField("最大尺寸_高:", _config._nMaxHeight);
                //GUILayout.EndHorizontal();

                _config._eAlgorithm = (SpriteToolConfigData.Algorithm)EditorGUILayout.EnumPopup("Algorithm:", _config._eAlgorithm);

                //// 自动使用TrueColor
                //_config._bPackAutoTrueColor = GUILayout.Toggle(_config._bPackAutoTrueColor, "打出的图集自动使用TrueColor");

                //// 自动不使用mipmap
                //_config._bPackAutoNoMipmap = GUILayout.Toggle(_config._bPackAutoNoMipmap, "打出的图集不使用mipmap");


                GUILayout.EndVertical();
            }

        }
        void OnGUI()
        {
            Show_Config(config);
        }

        void Show_OutPut(SpriteToolConfigData config)
        {
            GUILayout.Space(10);
            GUILayout.BeginVertical();
        }



        #endregion


        #region _SpriteTool_

        const string strMenuTitle_SliceSprite = "Assets/SpritesTool/图集分解";
        //const string strMenuTitle_PackSprite = "Assets/SpritesTool/打图集到当前文件夹";
        //const string strMenuTitle_packSpriteToFolder = "Assets/SpritesTool/打图集到目标文件夹";
        const string strMenuTitle_PackSprite = "Assets/SpritesTool/打图集";
        const string strMenuTitle_copyFolderPath = "Assets/SpritesTool/复制文件夹路径";

        static bool CheckIsMultipleSprite()
        {
            // 选中的是一张图片，该图片类型为Sprite，且为multiSprite
            Object[] os = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
            if (os == null
                || os.Length == 0)
            {
                return false;
            }
            return true;
        }
        static bool CheckIsFolder()
        {
            Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            if (os.Length != 1)
            {
                return false;
            }
            string filePath = AssetDatabase.GetAssetPath(os[0].GetInstanceID());
            if (!Directory.Exists(filePath))
            {
                return false;
            }
            return true;
        }


        #region _图集分解_
        [MenuItem(strMenuTitle_SliceSprite, true)]
        private static bool Check_SliceSprite()
        {
            return CheckIsMultipleSprite();
        }
        [MenuItem(strMenuTitle_SliceSprite)]
        static void SpriteSliceToPngs()
        {
            // 打开对话框，选择保存路径
            string strOutFolderPath = EditorUtility.OpenFolderPanel("选择输出文件夹", Application.dataPath, "");
            if (strOutFolderPath.Length == 0)
            {
                return;
            }

            Object[] os = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
            for (int i = 0; i < os.Length; ++i)
            {
                string path = AssetDatabase.GetAssetPath(os[i]);
                SliceSprite(path, strOutFolderPath);
            }
        }

        /// <summary>
        /// 将strAssetPath的图片分解到strOutFolderAbsPath文件夹中
        /// </summary>
        /// <param name="strAssetPath">要分解的图片的Assets路径，即以"Assets/"开头的路径</param>
        /// <param name="strOutFolderAbsPath">保存分解图片的文件夹路径，是个绝对路径</param>
        static void SliceSprite(string strAssetPath, string strOutFolderAbsPath)
        {
            Texture2D myTexture = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>(strAssetPath);
            if (myTexture == null)
            {
                Debug.LogWarning("<color=orange>[Warning]</color>---" + strAssetPath + " is Not Texture2D");
                return;
            }
            string path = AssetDatabase.GetAssetPath(myTexture);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            bool bOldReadTag = ti.isReadable;
            ti.isReadable = true;
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();


            SpriteMetaData[] dates = ti.spritesheet;
            if (dates == null
                || dates.Length == 0)
            {
                EditorUtility.DisplayDialog("错误！", "选中的资源格式错误或没有子图片！", "好吧，我错了");
                Debug.LogError("<color=red>[Error]</color>---" + strAssetPath + "Sprite ERROR : NULL or LengthZero");
                return;
            }

            // 生成Assets路径
            string strOutFolderAssetPath = strOutFolderAbsPath.ToLower().Replace(Application.dataPath.ToLower(), "assets");

            for (int i = 0; i < dates.Length; ++i)
            {
                Texture2D tex = new Texture2D((int)dates[i].rect.width, (int)dates[i].rect.height);
                tex.SetPixels(myTexture.GetPixels((int)dates[i].rect.x, (int)dates[i].rect.y, (int)dates[i].rect.width, (int)dates[i].rect.height));
                // 最后将这些纹理数据，成一个png图片文件  
                byte[] bytes = tex.EncodeToPNG();
                string filename = strOutFolderAbsPath + "/" + dates[i].name + ".png";
                System.IO.File.WriteAllBytes(filename, bytes);
                AssetDatabase.Refresh();

                // 修改图片属性
                Texture2D texSub = (Texture2D)AssetDatabase.LoadAssetAtPath<Texture2D>(strOutFolderAssetPath + "/" + dates[i].name + ".png");
                string pathSub = AssetDatabase.GetAssetPath(texSub);
                TextureImporter tii = AssetImporter.GetAtPath(pathSub) as TextureImporter;
                tii.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                tii.mipmapEnabled = false;
                tii.textureType = TextureImporterType.Sprite;
                tii.spriteImportMode = SpriteImportMode.Single;
                tii.spritePixelsPerUnit = 100;
                AssetDatabase.ImportAsset(pathSub);
            }

            ti.isReadable = bOldReadTag;
            AssetDatabase.ImportAsset(path);

            AssetDatabase.Refresh();
        }
        #endregion

        #region _打包图集_
        [MenuItem(strMenuTitle_PackSprite, true)]
        [MenuItem(strMenuTitle_copyFolderPath, true)]
        private static bool Check_PackSprite()
        {
            return CheckIsFolder();
        }

        [MenuItem(strMenuTitle_PackSprite)]
        static void SpritePackToOverWriter()
        {
            Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            if (os.Length != 1)
            {
                return;
            }
            string filePath = AssetDatabase.GetAssetPath(os[0].GetInstanceID());
            SelectOverWriterPNGWnd.AddWindow(filePath);
        }

        [MenuItem(strMenuTitle_copyFolderPath)]
        static void CopySelctFolderPath()
        {
            Object[] os = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            string packFolder = AssetDatabase.GetAssetPath(os[0].GetInstanceID()).ToLower();
            packFolder = packFolder.Remove(packFolder.IndexOf("assets/"), "assets/".Length);
            packFolder = Application.dataPath + "/" + packFolder;

            TextEditor t = new TextEditor();
            t.text = packFolder;
            t.SelectAll();
            t.Copy();

            Debug.Log("<color=green>" + "已复制：" + packFolder + "</color>");
        }

        #endregion




        #endregion
    }
}
