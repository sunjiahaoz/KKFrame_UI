using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace KK.Frame.UI
{
    public class UIControl_MessageBox : MonoBehaviour
    {
        public delegate void VOIDDELEGATE();

        public enum MessageBoxBtnMode
        {
            OK = 1 << 0,                     // 只有确定按钮
            Cancel = 1 << 1,                 // 只有取消按钮        
            Blank = 1 << 2,                  // 有空白点击的
            OKCancel = OK | Cancel,
            OKBlank = OK | Blank,
            CancelBlank = Cancel | Blank,
            OKCancelBlank = OKCancel | Blank,
        }
        public struct MessageBoxParam
        {
            public MessageBoxParam(
                MessageBoxBtnMode btnMode,
                string strTitle,
                string strContent,
                string strContentSmall,
                string strCancelText,
                string strConfirmText,
                bool bExpandWidth,
                bool bExpandHeight,
                VOIDDELEGATE actionClickConfirm,
                VOIDDELEGATE actionClickCancel,
                VOIDDELEGATE actionClickBlank
                )
            {
                _nBtnmode = (int)btnMode;
                _strTitle = strTitle;
                _strContent = strContent;
                _strContentSmall = strContentSmall;
                _strCancelText = strCancelText;
                _strConfirmText = strConfirmText;
                _action_click_cancel = actionClickCancel;
                _action_click_confirm = actionClickConfirm;
                _action_click_blank = actionClickBlank;

                if (_strCancelText.Length == 0)
                {
                    _strCancelText = "Cancel";
                }
                if (_strConfirmText.Length == 0)
                {
                    _strConfirmText = "Ok";
                }

                _bExpandWidth = bExpandWidth;
                _bExpandHeight = bExpandHeight;
            }
            public string _strTitle;                    // 标题
            public string _strContent;              // 内容
            public string _strContentSmall;     // 附加内容
            public VOIDDELEGATE _action_click_confirm;  // 点击确定执行动作
            public VOIDDELEGATE _action_click_cancel;    // 点击取消执行动作
            public VOIDDELEGATE _action_click_blank;      // 点击空白处执行动作
            public string _strCancelText;                               // 取消按钮文字
            public string _strConfirmText;                              // 确定按钮文字
            public int _nBtnmode;                     // 按钮mode
            public bool _bExpandWidth;      // 扩展宽度（当文字超出一定宽度就扩大边框）
            public bool _bExpandHeight;     // 扩展高度（当文字超出一定高度就扩大边框）

            // 常用默认值
            public static MessageBoxParam OK = new MessageBoxParam(MessageBoxBtnMode.OK | MessageBoxBtnMode.Blank, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false, true, null, null, null);
            public static MessageBoxParam Cancel = new MessageBoxParam(MessageBoxBtnMode.Cancel | MessageBoxBtnMode.Blank, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false, true, null, null, null);
            public static MessageBoxParam OKCancel = new MessageBoxParam(MessageBoxBtnMode.OKCancel | MessageBoxBtnMode.Blank, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, false, true, null, null, null);

        }

        public GameObject _btnConfirm;  // 确定按钮
        public GameObject _btnCancel;   // 取消按钮
        public GameObject _btnCenter;   // 只显示一个按钮时的那个按钮
        public GameObject _btnBlank;    // 空白
        public Text _lbConfirmText; // 确定按钮的文字
        public Text _lbCancelText;  // 取消按钮的文字
        public Text _lbCenterText;  // 中间按钮的文字
        public Text _lbContent;     // 内容文字
        public Text _lbTitle;       // 标题文字
        public Text _lbContentSmall;    // 额外内容文字    
        public ContentSizeFitter _expandFitter;

        protected MessageBoxParam _param;

        public virtual void ShowMessageBox(MessageBoxParam param)
        {
            Init(param);
        }

        protected virtual void Init(MessageBoxBtnMode btnMode,
            string strTitle, string strContent,
            string strContentSmall, string strCancelText, string strConfirmText,
            VOIDDELEGATE actionClickConfirm,
            VOIDDELEGATE actionClickCancel,
            VOIDDELEGATE actionClickBlank = null
            )
        {
            MessageBoxParam param = new MessageBoxParam();
            param._nBtnmode = (int)btnMode;
            param._strTitle = strTitle;
            param._strContent = strContent;
            param._strContentSmall = strContentSmall;
            param._strCancelText = strCancelText;
            param._strConfirmText = strConfirmText;
            param._action_click_cancel = actionClickCancel;
            param._action_click_blank = actionClickBlank;
            param._action_click_confirm = actionClickConfirm;
            Init(param);
        }

        protected void Init(MessageBoxParam param)
        {
            _param = param;
            SetBtnMode(param._nBtnmode);
            SetLabelText(_lbTitle, param._strTitle);
            SetLabelText(_lbContent, param._strContent);
            SetLabelText(_lbContentSmall, param._strContentSmall);
            SetLabelText(_lbConfirmText, param._strConfirmText);
            SetLabelText(_lbCancelText, param._strCancelText);
            if ((param._nBtnmode & (int)MessageBoxBtnMode.OK) != 0)
            {
                SetLabelText(_lbCenterText, param._strConfirmText);
            }
            if ((param._nBtnmode & (int)MessageBoxBtnMode.Cancel) != 0)
            {
                SetLabelText(_lbCenterText, param._strCancelText);
            }

            SetExpandFitter(param._bExpandWidth, param._bExpandHeight);
        }

        void SetExpandFitter(bool bExpandWidth, bool bExpandHeight)
        {
            if (_expandFitter != null)
            {
                _expandFitter.horizontalFit = bExpandWidth ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
                _expandFitter.verticalFit = bExpandHeight ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
            }
        }

        // 点击确定按钮处理
        public virtual void OnBnClickConfirm()
        {
            if (_param._action_click_confirm != null)
            {
                _param._action_click_confirm();
            }
        }
        // 点击取消按钮处理
        public virtual void OnBnClickCancel()
        {
            if (_param._action_click_cancel != null)
            {
                _param._action_click_cancel();
            }
        }
        // 点击空白处的处理
        public virtual void OnBnClickBlank()
        {
            if (_param._action_click_blank != null)
            {
                _param._action_click_blank();
            }
        }
        // 当btnMode为OK或Cancel的时候，点击的中间的按钮
        public virtual void OnBnClickCenter()
        {
            if ((_param._nBtnmode & (int)MessageBoxBtnMode.OK) != 0)
            {
                OnBnClickConfirm();
            }
            else if ((_param._nBtnmode & (int)MessageBoxBtnMode.Cancel) != 0)
            {
                OnBnClickCancel();
            }
        }

        // 按钮模式
        protected virtual void SetBtnMode(int nBtnMode)
        {
            _btnConfirm.SetActive(false);
            _btnCancel.SetActive(false);
            _btnCenter.SetActive(false);
            _btnBlank.SetActive(false);

            // 两个按钮
            if ((nBtnMode & (int)MessageBoxBtnMode.Cancel) != 0
                && (nBtnMode & (int)MessageBoxBtnMode.OK) != 0)
            {
                _btnConfirm.SetActive(true);
                _btnCancel.SetActive(true);
                _btnCenter.SetActive(false);
            }
            // 只有一个按钮
            else if ((nBtnMode & (int)MessageBoxBtnMode.Cancel) != 0
                || (nBtnMode & (int)MessageBoxBtnMode.OK) != 0)
            {
                _btnCenter.SetActive(true);
            }
            // 空白
            if ((nBtnMode & (int)MessageBoxBtnMode.Blank) != 0)
            {
                _btnBlank.SetActive(true);
            }
        }
        protected virtual void SetLabelText(Text label, string strText)
        {
            if (label != null)
            {
                label.text = strText;
            }
        }
    }
}
