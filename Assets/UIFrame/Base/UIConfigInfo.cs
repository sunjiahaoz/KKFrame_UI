using UnityEngine;
using System.Collections;

namespace KK.Frame.UI
{
    [System.Serializable]
    public class UIConfigInfo
    {
        // 预设
        public UIFrame _prefabFrame;
        // 窗口ID
        public string _strFrameID;
        // 实例化时所在UIRoot的Pannel
        public int _nPannelIndex;        
    }
}
