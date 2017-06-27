using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace KK.Frame.Util.SpritePacker
{

    public class SelectOverWriterPNGWnd : EditorWindow
    {

        // 预设的打包参数
        public enum DefaultPackType
        {
            Default,
            RGBA8888,
            RGBA5555,
            RGBA4444,
            RGB888,
            RGB565,
            ALPHA,
            ALPHA_INTENSITY,
        }

        [MenuItem("Assets/SpritesTool/配置")]
        public static void AddWindow(string strPackFolderAbsPath)
        {
            SelectOverWriterPNGWnd window = (SelectOverWriterPNGWnd)EditorWindow.GetWindow<SelectOverWriterPNGWnd>("覆盖图集");
            window.Init(strPackFolderAbsPath);
            window.Show();
        }

        GUIStyle _style = new GUIStyle();
        string _strPackFolderAbsPath = string.Empty;
        Texture2D _owObj = null;
        DefaultPackType _packType = DefaultPackType.Default;
        public void Init(string strPackFolderAbsPath)
        {
            _style.fontSize = 12;
            _style.richText = true;
            _strPackFolderAbsPath = strPackFolderAbsPath;
        }

        void OnGUI()
        {
            Show_Data();
        }

        void Show_Data()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("打包的文件夹：");
            if (!Directory.Exists(_strPackFolderAbsPath))
            {
                GUILayout.Label("<color=red>路径不存在:" + _strPackFolderAbsPath + "</color>", _style);
                return;
            }
            else
            {
                GUILayout.Label("<color=green>" + _strPackFolderAbsPath + "</color>", _style);
            }

            _packType = (DefaultPackType)EditorGUILayout.EnumPopup("打包预设：", _packType);


            _owObj = EditorGUILayout.ObjectField(_owObj, typeof(Texture2D), false) as Texture2D;


            string strTitle = "hehe";
            if (_owObj == null)
            {
                strTitle = "-*创建图集*-";
            }
            else
            {
                strTitle = "=@覆盖图集@=";
            }

            if (GUILayout.Button(strTitle, GUILayout.Width(100)))
            {
                PackSprites(_strPackFolderAbsPath, _packType);
            }

            GUILayout.EndVertical();
        }

        string TranslateAssetPathToAbspath(string strAssetPath)
        {
            string strPath = strAssetPath.ToLower();
            strPath = strPath.Remove(strPath.IndexOf("assets/"), "assets/".Length);
            strPath = Application.dataPath + "/" + strPath;
            return strPath;
        }

        private void PackSprites(string strPackFolderAbsPath, DefaultPackType packType)
        {
            // 检测配置是否有
            SpriteToolConfigData configData = AssetDatabase.LoadAssetAtPath<SpriteToolConfigData>(SpriteToolConfigData._strConfigAssetPath);
            if (configData == null
                || !File.Exists(configData._strTPExePath))
            {
                if (EditorUtility.DisplayDialog("错误", "请先配置TexturPacker.exe的路径！", "去配置", "Cancel"))
                {
                    SpriteToolConfigEditor.AddWindow();
                }
                return;
            }

            // 打包的图集所在文件夹
            string packFolder = strPackFolderAbsPath;

            string strTpsheetPath = string.Empty;
            string strPngPath = string.Empty;
            if (_owObj == null)
            {
                // 选择保存路径
                string strTargetPath = EditorUtility.SaveFilePanel("保存", Application.dataPath, Path.GetFileNameWithoutExtension(strPackFolderAbsPath), "png");
                if (strTargetPath.Length == 0)
                {
                    return;
                }
                string strSaveFileName = Path.GetFileNameWithoutExtension(strTargetPath);
                strTargetPath = Path.GetDirectoryName(strTargetPath);

                strTpsheetPath = strTargetPath + "/" + strSaveFileName + ".txml";
                strPngPath = strTargetPath + "/" + strSaveFileName + ".png";
            }
            else
            {
                string strOWObjAssetPath = AssetDatabase.GetAssetPath(_owObj);
                // 源文件所在文件夹
                string strOWFileFolder = Path.GetDirectoryName(TranslateAssetPathToAbspath(strOWObjAssetPath));
                // 源文件名称，没有后缀名
                string strOWPngFileName = Path.GetFileNameWithoutExtension(strOWObjAssetPath);

                strTpsheetPath = strOWFileFolder + "/" + strOWPngFileName + ".txml";
                strPngPath = strOWFileFolder + "/" + strOWPngFileName + ".png";
            }


            // 先删除当前已经存在的同名文件
            if (File.Exists(strTpsheetPath))
            {
                File.Delete(strTpsheetPath);
            }
            if (File.Exists(strPngPath))
            {
                File.Delete(strPngPath);
            }
            //   string commandText = " --sheet {0}.png --data {1}.xml --format sparrow --trim-mode None --pack-mode Best  --algorithm MaxRects --max-size 2048 --size-constraints POT  --disable-rotation --scale 1 {2}";
            string strPackCmd = "--sheet {0} {1} --data {2} --format {3} --trim-mode {4} --pack-mode {5} --max-width {6} ";
            strPackCmd += "--max-height {7} --algorithm {8} --size-constraints {9} --opt {10}";
            strPackCmd = string.Format(strPackCmd,
                strPngPath,
                packFolder,
                strTpsheetPath,
                "sparrow",
                "None",
                "Best",
                configData._nMaxWidth,
                configData._nMaxWidth,
                configData._eAlgorithm,
                "POT",
                 GeneratePackExtraParam(packType)
                );
            // 生成执行参数
            //         string strPackCmd = string.Format("--data {0} --format sparrow --trim-mode None --pack-mode Best  --sheet {1} {2} --max-width {3} --max-height {4} --algorithm {5} {6}",
            //             strTpsheetPath,
            //             strPngPath,
            //             packFolder,
            //             configData._nMaxWidth, configData._nMaxHeight,
            //             configData._eAlgorithm.ToString(),
            //             GeneratePackExtraParam(packType));
            Debug.Log("<color=green>参数:" + strPackCmd + "</color>");

            // 执行命令行
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = configData._strTPExePath;
                p.StartInfo.Arguments = strPackCmd;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("<color=red>[Error]</color>---" + "Process Error : " + ex.Message);
                EditorUtility.DisplayDialog("执行出错", "执行出错：" + ex.Message, "好吧");
            }
        }

        string GeneratePackExtraParam(DefaultPackType packType)
        {
            switch (packType)
            {
                case DefaultPackType.Default:
                    return "";
                case DefaultPackType.RGBA8888:
                case DefaultPackType.RGBA5555:
                case DefaultPackType.RGBA4444:
                case DefaultPackType.RGB888:
                case DefaultPackType.RGB565:
                    return string.Format("{0} --dither-fs", packType.ToString());
                case DefaultPackType.ALPHA:
                case DefaultPackType.ALPHA_INTENSITY:
                    return string.Format("{0} --dither-fs-alpha", packType.ToString());
            }
            return "";
        }
    }
}
