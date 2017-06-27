using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KK.Frame.UI
{
    public partial class UIController : SingletonMonoBehaviour<UIController>
    {
        #region _无论主副都会使用到的数据_
        // 为false则是主C，true就是副C，全局只能有而且必须有一个主C
        public bool _isAdditional = false;        
        // 通过属性面板编辑
        public UIFrameItem[] _frameItems;
        #endregion

        #region _Controller的独一份数据，也就是说不会使用到副Controller中这些数据_
        /// <summary>
        /// 主C必须需要一个UIRoot
        /// </summary>
        public UIRoot _uiRoot;
        UIRoot uiRoot
        {
            get
            {
                return _uiRoot;
            }
        }

        // 保存所有窗口配置信息
        Dictionary<string, UIFrameItem> _dictItems;
        Dictionary<string, UIFrameItem> dictItems
        {
            get { return Instance._dictItems; }
        }
        // 当前已经显示过的窗口
        Dictionary<string, UIFrame> _dictLoadedFrame;
        Dictionary<string, UIFrame> dictLoadedFrame
        {
            get { return Instance._dictLoadedFrame; }
        }
        #endregion

        #region _只有副C才使用到的数据_        
        // 当Destroy的时候是否移除item信息
        public bool _bRemoveWhenDestroy = true;                
        #endregion

        #region _Inner_
        // 初始化主C
        void AwakeSingleton()
        {
            if (_uiRoot == null)
            {
                Debug.LogError("<color=red>[Error]</color>---" + "uiRoot没有设置！！", this);
            }
            Instance._dictItems = new Dictionary<string, UIFrameItem>();
            Instance._dictLoadedFrame = new Dictionary<string, UIFrame>();
            _InitDictItems(dictItems);
        }
        // 初始化副C
        void AwakeAdditional()
        {
            _InitDictItems(dictItems);
        }

        // 主C销毁时处理
        void OnDestroy_Singleton()
        { }
        // 副C销毁时处理
        void OnDestroy_Additional()
        {
            if (_bRemoveWhenDestroy)
            {
                _RemoveFromeDictItems(dictItems);                
            }
        }

        // 将frameItem添加到指定dictItem中
        void _InitDictItems(Dictionary<string, UIFrameItem> dictItems)
        {            
            for (int i = 0; i < _frameItems.Length; ++i)
            {
                if (dictItems.ContainsKey(_frameItems[i]._strFrameID))
                {
                    Debug.LogWarning("<color=orange>[Warning]</color>---窗口" + _frameItems[i]._strFrameID + "已经存在，不能重复加入");
                    continue;
                }
                dictItems.Add(_frameItems[i]._strFrameID, _frameItems[i]);
            }
        }
        // 将frameItem从指定dictItem中移除
        // 移除会先销毁已经加载过的窗口
        void _RemoveFromeDictItems(Dictionary<string, UIFrameItem> dictItems)
        {
            for (int i = 0; i < _frameItems.Length; ++i)
            {
                // 如果已经加载过UI，先删掉
                DestroyUI(_frameItems[i]._strFrameID);
                dictItems.Remove(_frameItems[i]._strFrameID);
            }
        }
        #endregion

        #region _StaticMethod_
        public static void ShowUI(string strFrameId, System.Action<UIFrame> actionAfterShow = null)
        {
            Instance._ShowUI(strFrameId, actionAfterShow);
        }
        public static void HideUI(string strFrameId)
        {
            Instance._HideUI(strFrameId);
        }
        public static void HideAll(int nPannelIndex = -1)
        {
            Instance._HideAll(nPannelIndex);
        }
        public static UIFrame GetFrame(string strFrameId)
        {
            return Instance._GetFrame(strFrameId);
        }
        public static void DestroyUI(string strFrameId)
        {
            Instance._DestroyUI(strFrameId);
        }
        public static void DestroyAll()
        {
            Instance._DestroyAll();
        }
        #endregion

        #region _Method_
        /// <summary>
        /// 显示UI，没有实例化过就先实例化
        /// </summary>
        /// <param name="strFrameId"></param>
        /// <param name="actionAfterShow"></param>
        void _ShowUI(string strFrameId, System.Action<UIFrame> actionAfterShow = null)
        {
            // 如果已经加载过了，直接显示吧
            if(dictLoadedFrame.ContainsKey(strFrameId))
            {                
                dictLoadedFrame[strFrameId].OnFrameShow();
                if (actionAfterShow != null)
                {
                    actionAfterShow(dictLoadedFrame[strFrameId]);
                }
                return;
            }

            // 判断配置是否存在
            if (!dictItems.ContainsKey(strFrameId))
            {              
                Debug.LogError("<color=red>[Error]</color>---" + "找不到窗口:" + strFrameId);  
                return;
            }

            // 实例化窗口
            UIFrame frame = GameObject.Instantiate(dictItems[strFrameId]._prefabFrame) as UIFrame;
            if (frame != null)
            {
                frame.FrameId = strFrameId;
                frame.OnFrameLoad();
                frame.transform.SetParent(uiRoot.GetPannel(dictItems[strFrameId]._nPannelIndex).transform);
                frame.transform.localScale = Vector3.one;
                frame.transform.localPosition = Vector3.zero;
                dictLoadedFrame.Add(strFrameId, frame);
                frame.OnFrameShow();

                if (actionAfterShow != null)
                {
                    actionAfterShow(dictLoadedFrame[strFrameId]);
                }
            }
        }
        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="strFrameId"></param>
        void _HideUI(string strFrameId)
        {
            if (!dictLoadedFrame.ContainsKey(strFrameId))
            {
                Debug.LogWarning("<color=orange>[Warning]</color>---" + "找不到窗口" + strFrameId + ", Hide失败");
                return;
            }
            dictLoadedFrame[strFrameId].OnFrameHide();
        }
        // 隐藏某个层级下的界面，如果参数为-1表示隐藏所有层级界面
        void _HideAll(int nPannelIndex = -1)
        {
            using (Dictionary<string, UIFrame>.Enumerator tor = dictLoadedFrame.GetEnumerator())
            {
                while (tor.MoveNext())
                {
                    if (nPannelIndex == -1
                        || dictItems[tor.Current.Value.FrameId]._nPannelIndex == nPannelIndex)
                    {
                        tor.Current.Value.OnFrameHide();
                    }
                }
            }
        }
        /// <summary>
        /// 获取某个界面的句柄
        /// 如果该界面没有显示过，则返回null
        /// </summary>
        /// <param name="strFrameId"></param>
        /// <returns></returns>
        UIFrame _GetFrame(string strFrameId)
        {
            if (!dictLoadedFrame.ContainsKey(strFrameId))
            {
                return null;
            }
            return dictLoadedFrame[strFrameId];
        }
        void _DestroyUI(string strFrameId)
        {
            if (!dictLoadedFrame.ContainsKey(strFrameId))
            {
                return;
            }
            dictLoadedFrame[strFrameId].OnFrameDestroy();
            GameObject.DestroyImmediate(dictLoadedFrame[strFrameId].gameObject);
            dictLoadedFrame.Remove(strFrameId);
        }
        void _DestroyAll()
        {
            using (Dictionary<string, UIFrame>.Enumerator tor = dictLoadedFrame.GetEnumerator())
            {
                while (tor.MoveNext())
                {
                    tor.Current.Value.OnFrameDestroy();
                    GameObject.DestroyImmediate(tor.Current.Value.gameObject);                    
                }
            }
            dictLoadedFrame.Clear();
        }
        #endregion

        #region _SingletonMonoBehaviour_

        protected override void Awake()
        {
            base.Awake();
            if (_isAdditional)
            {
                AwakeAdditional();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!_isAdditional)
            {
                OnDestroy_Singleton();
            }
            else
            {
                OnDestroy_Additional();
            }
        }

        public override bool isSingletonObject
        {
            get
            {
                return !_isAdditional;
            }
        }
        #endregion               
    }
}
