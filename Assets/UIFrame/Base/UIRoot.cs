/********************************************************************
	created:	2017/06/12 		
	file base:	UIRoot.cs	
	author:		sunjiahaoz
	
	purpose:	描述窗口所依附的Pannel,Canvas之类的
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KK.Frame.UI
{
    public class UIRoot : MonoBehaviour
    {
        public Transform PannelRoot;

        List<UIPannel> _lstPannel = new List<UIPannel>();

        Camera _uiCam;
        public Camera uiCam
        {
            get
            {
                if (_uiCam == null)
                {
                    _uiCam = GetComponent<Camera>();
                }
                return _uiCam;
            }
        }

        void Awake()
        {
            _lstPannel.Clear();
            UIPannel[] pannels = PannelRoot.GetComponentsInChildren<UIPannel>();
            _lstPannel.AddRange(pannels);

            // 排序，list中越靠前表示层级越低
            _lstPannel.Sort((p1, p2) =>
            {
                // 如果在同一层
                if (p1.canvas.sortingLayerName.CompareTo(p2.canvas.sortingLayerName) == 0)
                {
                    return p1.canvas.sortingOrder.CompareTo(p2.canvas.sortingOrder);
                }
                else
                {
                    int nId1 = SortingLayer.NameToID(p1.canvas.sortingLayerName);
                    int nId2 = SortingLayer.NameToID(p2.canvas.sortingLayerName);
                    return -nId1.CompareTo(nId2);
                }
            });
        }

        public UIPannel GetPannel(int nIndex)
        {
            if (nIndex < 0
                || nIndex >= _lstPannel.Count)
            {
                return null;
            }
            return _lstPannel[nIndex];
        }
    }
}
