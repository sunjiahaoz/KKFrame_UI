using UnityEngine;
using System.Collections;
namespace KK.Frame.Util.SpritePacker
{

    public class SpriteToolConfigData : ScriptableObject
    {
        public const string _strConfigAssetPath = "Assets/KKFrame/KKFrameUtil/Editor/SpriteTool/SpriteToolConfig.asset";

        public enum Algorithm
        {
            MaxRects,
            Basic,
        }

        [SerializeField]
        public string _strTPExePath = "";  // TPexe的路径
        public int _nMaxWidth = 1024;
        public int _nMaxHeight = 1024;

        public Algorithm _eAlgorithm = Algorithm.MaxRects;
        //[SerializeField]
        //public bool _bPackAutoTrueColor = true;    // 打包图集自动设置为trueColor
        //[SerializeField]
        //public bool _bPackAutoNoMipmap = true; // 打包图集自动取消设置mipmap
    }

}
