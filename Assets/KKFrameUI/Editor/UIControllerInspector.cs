using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
namespace KK.Frame.UI
{
    [CustomEditor(typeof(UIController))]
    
    public class UIControllerInspector : Editor
    {
        int index = 0;          //数组角标
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            UIController uc = target as UIController;
            EditorGUILayout.BeginVertical();
            uc._isAdditional = EditorGUILayout.Toggle("Is Additional", uc._isAdditional);           //isAdditional开关
            if (uc._isAdditional)
            {
                uc._bRemoveWhenDestroy = EditorGUILayout.Toggle("Remove When Destroy", uc._bRemoveWhenDestroy);
            }
            else
            {
                uc._uiRoot = (UIRoot)EditorGUILayout.ObjectField(uc._uiRoot, typeof(UIRoot), true);
            }
            EditorGUILayout.LabelField("Please Lock The Inspector", GUILayout.Width(300));      
      
            if(GUILayout.Button("Add Selected Frame To UIController",GUILayout.Height(20)))     //添加新的Frame的按钮
            {
               int i =  AddNewFrame(uc);
               if (i >= 0) 
               {
                   index = i;
               }
            }
            
            if (uc._frameItems.Length != 0)
            {
                string[] _saFrameItemName = new string[uc._frameItems.Length];
                for (int i = 0; i < uc._frameItems.Length; i++)
                {
                    _saFrameItemName[i] = uc._frameItems[i]._strFrameID;
                }
                EditorGUILayout.BeginHorizontal();
                index = EditorGUILayout.Popup("FrameItem", index, _saFrameItemName);    //菜单栏
                if (GUILayout.Button("一", GUILayout.Width(30)))                 //删掉当前选中Frame的按钮
                {
                    RemoveFrame(uc,index);
                    index = 0;
                    if (uc._frameItems.Length == 0) 
                    {
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
                    uc._frameItems[index]._strFrameID = EditorGUILayout.TextField("FrameId", uc._frameItems[index]._strFrameID);
                    uc._frameItems[index]._nPannelIndex = EditorGUILayout.IntField("PannelIndex", uc._frameItems[index]._nPannelIndex);
                    uc._frameItems[index]._prefabFrame = (UIFrame)EditorGUILayout.ObjectField("Prefab", uc._frameItems[index]._prefabFrame, typeof(UIFrame), true);     
             }

            EditorGUILayout.EndVertical();
            Undo.RecordObject(target,"tar change");         //用于保存编辑的数据
        }
        private void RemoveFrame(UIController uc,int index) 
        {
            uc._frameItems[index] = null;
            List<UIFrameItem> li = ArrayConverseToList(uc._frameItems);
            uc._frameItems = ListConverseToArray(li);
        }
        private int AddNewFrame(UIController uc)
        {
            Object[] selection = (Object[])Selection.objects;
            if (selection.Length == 0)
            {
                return -1;
            }
            List<UIFrameItem> _liEditUse = ArrayConverseToList(uc._frameItems);
            foreach (Object obj in selection)
            {               
                UIFrameItem ufi = new UIFrameItem();
                GameObject go = (GameObject)obj;
                UIFrame uf = go.GetComponent<UIFrame>();
                ufi._prefabFrame = uf;
                uf.FrameId = "ID_" + uf.gameObject.name;
                ufi._strFrameID = uf.FrameId;
                ufi._nPannelIndex = 1;
                _liEditUse.Add(ufi);
            }
            uc._frameItems = ListConverseToArray(_liEditUse);
            return _liEditUse.Count - 1;
        }
        private UIFrameItem[] ListConverseToArray(List<UIFrameItem> li)
        {
            UIFrameItem[] ufiarray = new UIFrameItem[li.Count];
            for (int i = 0; i < li.Count;i++ )
            {
                ufiarray[i] = li[i];
            }
            return ufiarray;
        }
        private List<UIFrameItem> ArrayConverseToList(UIFrameItem[] _uiArray)
        {
            List<UIFrameItem> _lUiList = new List<UIFrameItem>();
            for (int i = 0; i < _uiArray.Length; i++)
            {
                if (_uiArray[i] != null)            //把不为null的数组都添加到新建的链表中
                {
                    _lUiList.Add(_uiArray[i]);
                }
            }
            return _lUiList;
        }
        //void OnInspectorUpdate()
        //{
        //    Repaint();
        //}
    }   
}
