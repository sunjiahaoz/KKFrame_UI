using UnityEngine;
using UnityEditor;
using System.Collections;

namespace KK.Frame.UI
{
    [CustomEditor(typeof(UIController))]
    public class UIControllerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label("hellow");
        }
    }
}
