using UnityEngine;
using System.Collections;

namespace KK.Frame.UI
{
    public class UIFrame : MonoBehaviour
    {
        public virtual string FrameId { get; set; }        

        /// <summary>
        /// 资源加载完
        /// 同一个IFrame应该只执行一次该函数
        /// </summary>
        public virtual void OnFrameLoad() { }
        /// <summary>        
        /// 由隐藏状态转入显示的时候执行        
        /// </summary>
        public virtual void OnFrameShow()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// 由显示状态转入隐藏的时候执行
        /// </summary>
        public virtual void OnFrameHide()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// 资源销毁
        /// 同一个IFrame应该只执行一次该函数
        /// </summary>
        public virtual void OnFrameDestroy() { }
    }
}
