using UnityEngine;
using System.Collections;
using UnityEditor;

namespace KK.Frame.Util.SpritePacker
{
    public class AtlasPostProcesser : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] moveToPaths, string[] moveFromPaths)
        {

            for (int i = 0; i < importedAssets.Length; ++i)
            {
                string strExt = System.IO.Path.GetExtension(importedAssets[i]);                
                if (strExt.CompareTo(".txml") == 0)
                {
                    ToSpriter(importedAssets[i]);
                }
            }
        }

        private static void ToSpriter(string strFile)
        {
            SegmentationTexture.SegmentationTextureToSprite_ByXmlPath(strFile);
        }
    }
}
