using UnityEngine;
using System.Collections;
using KK.Frame.UI;

public class Scene2 : MonoBehaviour {
    void OnGUI()
    {
        if (GUILayout.Button("Click", GUILayout.Width(100), GUILayout.Height(100)))
        {
            UIController.ShowUI("ID_FrameTest2_1");
        }
        if (GUILayout.Button("ToThird", GUILayout.Width(100), GUILayout.Height(100)))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Third");
        }
    }
}
