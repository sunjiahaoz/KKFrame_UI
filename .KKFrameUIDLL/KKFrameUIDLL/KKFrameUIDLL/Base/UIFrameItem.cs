using UnityEngine;
using System.Collections;

namespace KK.Frame.UI
{
    [System.Serializable]
    public class UIFrameItem
    {
        /// <summary>
        /// 预设
        /// </summary>
        public UIFrame _prefabFrame;
        /// <summary>
        /// 窗口ID
        /// </summary>
        public string _strFrameID;
        /// <summary>
        /// 实例化时所在UIRoot的Pannel
        /// </summary>
        public int _nPannelIndex;        
    }
}
