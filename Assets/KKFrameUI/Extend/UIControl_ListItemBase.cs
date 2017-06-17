using UnityEngine;
using System.Collections;
namespace KK.Frame.UI
{

    public class UIControl_ListItemBase : MonoBehaviour
    {
        protected int _nData = -1;
        public virtual void SetItemData(int nData)
        {
            _nData = nData;
        }

        public virtual int GetItemData()
        {
            return _nData;
        }

        public virtual float GetHeight()
        { return 0; }
        public virtual float GetWidth()
        { return 0; }
    }
}
