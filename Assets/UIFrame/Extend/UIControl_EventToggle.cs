using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using KK.Frame.Util;

namespace  KK.Frame.UI
{

    [RequireComponent(typeof(Toggle))]
    public class UIControl_EventToggle : MonoBehaviour
    {
        public int _nId = 0;
        [SerializeField]
        public ToggleChangedWithSelf OnToggleValueChangedWithSelf;

        protected Toggle _toggle;
        protected virtual Toggle toggle
        {
            get
            {
                if (_toggle == null)
                {
                    _toggle = ToolsUseful.DefaultGetComponent<Toggle>(gameObject);
                }
                return _toggle;
            }
        }

        #region _文字_
        public Text _lbToggle;
        // 选中与取消选中状态下的文字颜色
        public Color _colorToggleOn = Color.white;
        public Color _colorToggleOff = Color.white;
        #endregion

        #region _active_
        public Transform[] _activeToggleOn;    // 选中时打开这些对象，取消选中时会隐藏
        public Transform[] _inactiveToggleOn;   // 选中时隐藏这些对象，取消选中时会打开
        #endregion

        protected virtual void Awake()
        {
            if (toggle.onValueChanged == null)
            {
                toggle.onValueChanged = new Toggle.ToggleEvent();
            }
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        protected virtual void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        protected virtual void OnValueChanged(bool bValue)
        {
            OnToggleValueChangedWithSelf.Invoke(this, bValue);

            if (_lbToggle != null)
            {
                _lbToggle.color = bValue ? _colorToggleOn : _colorToggleOff;
            }
            if (_activeToggleOn != null)
            {
                for (int i = 0; i < _activeToggleOn.Length; ++i)
                {
                    _activeToggleOn[i].gameObject.SetActive(bValue);
                }
            }
            if (_inactiveToggleOn != null)
            {
                for (int i = 0; i < _inactiveToggleOn.Length; ++i)
                {
                    _inactiveToggleOn[i].gameObject.SetActive(!bValue);
                }
            }
        }

        [System.Serializable]
        public class ToggleChangedWithSelf : UnityEvent<UIControl_EventToggle, bool>
        {
            public ToggleChangedWithSelf() { }
        }
    }
}
