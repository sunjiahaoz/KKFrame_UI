using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KK.Frame.UI
{
    public partial class UIController : SingletonMonoBehaviour<UIController>
    {
        public UIRoot _uiRoot;

        #region _CommonSetting通用设置_
        public bool _isAdditional = false;
        #endregion



#region _Inner_
        void AwakeSingleton()
        {

        }
#endregion

        #region _Method_
        public void ShowUI(string strFrameId) { }
        public void HideUI(string strFrameId) { }
        #endregion

        #region _SingletonMonoBehaviour_

        protected override void Awake()
        {
            base.Awake();
            if (!_isAdditional)
            {
                AwakeSingleton();
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
