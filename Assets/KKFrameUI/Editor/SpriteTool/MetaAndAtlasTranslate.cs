using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class MetaAndAtlasTranslate
{

    [MenuItem("Window/Frame/将窗口中所有Image替换为使用图集的sprite")]
    public static void TranslateMetToAtlasAll()
    {
        GameObject go = Selection.activeGameObject;
        if (go == null)
        {
            Debug.LogWarning("<color=orange>[Warning]</color>---" + "没有选中任何对象");
            return;
        }

        Image[] imgs = go.GetComponentsInChildren<Image>(true);
        
        for (int i = 0; i < imgs.Length; ++i)
        {
            TranslateMetaToAtals(imgs[i]);
        }
    }

    [MenuItem("Window/Frame/将窗口中所有Image替换为使用散图的sprite")]
    public static void TranslateAtlasAllToMeta()
    {
        GameObject go = Selection.activeGameObject;
        if (go == null)
        {
            Debug.LogWarning("<color=orange>[Warning]</color>---" + "没有选中任何对象");
            return;
        }

        Image[] imgs = go.GetComponentsInChildren<Image>(true);

        for (int i = 0; i < imgs.Length; ++i)
        {
            TranslateAtlasToMeta(imgs[i]);
        }
    }

    private static Dictionary<string, UnityEngine.Object[]> _loaded;
    /// <summary>
    /// 将Image的sprite替换为图集中的同名sprite
    /// 需要满足以下规则，源sprite位于KKModel/Atlas/(图集同名文件夹名)下
    /// 图集位于KKModel/ForZip/Texture/图集名称.png    
    /// </summary>
    /// <param name="image"></param>
    private static void TranslateMetaToAtals(Image image)
    {
        if (image == null)
        {
            return;
        }
        if (image.sprite == null)
        {
            Debug.LogWarning("<color=orange>[Warning]</color>---" + "Sprite is NULL", image);
            return;
        }
        string strSpriteName = image.sprite.name;
        // 获取sprite的路径
        string strSpritePath = AssetDatabase.GetAssetPath(image.sprite);
        Debug.Log("<color=green>[log]</color>---" + "spritePath:" + strSpritePath);

        // 判断是否是atlas文件夹下的
        if (!strSpritePath.ToLower().Contains("/atlas/"))
        {
            Debug.LogWarning("<color=orange>[Warning]</color>---" + image.sprite + "不是Atlas文件夹下的图片！！！", image);            
            return;
        }
        // 获取该图片应该在的图集名称
        string strAtlasName = Path.GetFileName(Path.GetDirectoryName(strSpritePath));
        // 获得图集应该在的路径
        string strTexturePath = Path.GetDirectoryName(strSpritePath.ToLower().Replace("atlas/", "ForZip/Texture/")) + ".png";
        // 判断该图集是否存在
        if (!File.Exists(strTexturePath))
        {
            Debug.LogWarning("<color=orange>[Warning]</color>---" + "图集" + strSpritePath + "不存在！！", image);
            return;
        }

        if (_loaded == null)
        {
            _loaded = new Dictionary<string, Object[]>();
        }
        UnityEngine.Object[] objs = null;
        if (_loaded.ContainsKey(strTexturePath))
        {
            objs = _loaded[strTexturePath];
        }
        else
        {
            objs = AssetDatabase.LoadAllAssetsAtPath(strTexturePath);
            _loaded.Add(strTexturePath, objs);
        }
        
        for (int i = 0; i < objs.Length; ++i)
        {
            if (strSpriteName.Equals(objs[i].name))
            {
                image.sprite = objs[i] as Sprite;
                Debug.Log("<color=green>"+"替换成功"+"</color>", image);
                break;
            }            
        }
    }    

    private static void TranslateAtlasToMeta(Image img)
    {
        if (img == null)
        {
            return;
        }
        if (img.sprite == null)
        {
            Debug.LogWarning("<color=orange>[Warning]</color>---" + "Sprite is NULL", img);
            return;
        }

        string strSpriteName = img.sprite.name;
        string strAtlasPath = AssetDatabase.GetAssetPath(img.sprite);
        string strFolder = Path.GetFileNameWithoutExtension(strAtlasPath);
        string strFolderPath = Path.GetDirectoryName(strAtlasPath);
        string strMetaPath = strFolderPath.ToLower().Replace("forzip/texture", "atlas/" + strFolder + "/") + strSpriteName + ".png";
        if (!File.Exists(strMetaPath))
        {
            Debug.LogWarning("<color=orange>[Warning]</color>---" + "找不到文件" + strMetaPath);
            return;
        }
        img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(strMetaPath);
    }


    [InitializeOnLoadMethod]
    static void StartInitializeOnLoadMethod()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        if (Event.current != null && selectionRect.Contains(Event.current.mousePosition)
            && Event.current.button == 1 && Event.current.type <= EventType.mouseUp)
        {
            GameObject selectedGameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            //这里可以判断selectedGameObject的条件
            if (selectedGameObject.GetComponent<KK.Frame.UI.UIFrame>())
            {
                Vector2 mousePosition = Event.current.mousePosition;

                EditorUtility.DisplayPopupMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), "Window/Frame", null);
                Event.current.Use();
            }
        }
    }
}
