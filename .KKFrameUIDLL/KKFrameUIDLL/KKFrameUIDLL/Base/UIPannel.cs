using UnityEngine;
using System.Collections;

namespace KK.Frame.UI
{
    public class UIPannel : MonoBehaviour
    {
        Canvas _canvas;
        public Canvas canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = GetComponent<Canvas>();
                }
                return _canvas;
            }
        }
    }
}
