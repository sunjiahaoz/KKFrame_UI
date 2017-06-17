/********************************************************************
	created:	2017/02/15 		
	file base:	UIControl_LinkText.cs	
	author:		sunjiahaoz
	
	purpose:	文本超链接
    需要一个underline的Text对象，可以建个子对象作为
*********************************************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace KK.Frame.UI
{

    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(Button))]
    public class UIControl_LinkText : MonoBehaviour
    {
        private Text linkText;
        private Button btn;
        public Text _underLine;

        void Awake()
        {
            linkText = GetComponent<Text>();
            btn = GetComponent<Button>();
            btn.transition = Selectable.Transition.None;
        }
        void Start()
        {
            CreateLink(linkText);
        }

        public void CreateLink(Text text)
        {
            if (text == null)
                return;

            //克隆Text，获得相同的属性          
            Text underline = _underLine;
            RectTransform rt = underline.rectTransform;

            //设置下划线坐标和位置  
            rt.anchoredPosition3D = Vector3.zero;
            rt.offsetMax = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchorMin = Vector2.zero;

            underline.text = "_";
            float perlineWidth = underline.preferredWidth;      //单个下划线宽度  
                                                                //Debug.Log(perlineWidth);

            float width = text.preferredWidth;
            //Debug.Log(width);
            int lineCount = (int)Mathf.Round(width / perlineWidth);
            //Debug.Log(lineCount);
            for (int i = 1; i < lineCount; i++)
            {
                underline.text += "_";
            }
        }
    }
}
