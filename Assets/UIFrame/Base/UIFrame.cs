using UnityEngine;
using System.Collections;

namespace KK.Frame.UI
{
    public abstract class UIFrame : MonoBehaviour
    {
        /// <summary>
        /// 资源加载完
        /// 同一个IFrame应该只执行一次该函数
        /// </summary>
        public abstract void OnFrameLoad();
        /// <summary>        
        /// 由隐藏状态转入显示的时候执行        
        /// </summary>
        public abstract void OnFrameShow();
        /// <summary>
        /// 由显示状态转入隐藏的时候执行
        /// </summary>
        public abstract void OnFrameHide();
        /// <summary>
        /// 资源销毁
        /// 同一个IFrame应该只执行一次该函数
        /// </summary>
        public abstract void OnFrameDestroy();
    }
}
