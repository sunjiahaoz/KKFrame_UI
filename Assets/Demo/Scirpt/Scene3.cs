using UnityEngine;
using System.Collections;
using KK.Frame.UI;

public class Scene3 : MonoBehaviour {

    void OnGUI()
    {
        if (GUILayout.Button("Click0", GUILayout.Width(100), GUILayout.Height(100)))
        {
            UIController.ShowUI("ID_FrameTest2_1");
        }
        if (GUILayout.Button("Click1", GUILayout.Width(100), GUILayout.Height(100)))
        {
            UIController.ShowUI("ID_FrameTest1");
        }
        if (GUILayout.Button("Clear", GUILayout.Width(100), GUILayout.Height(100)))
        {
            UIController.DestroyAll();
        }
        if (GUILayout.Button("ToSecond", GUILayout.Width(100), GUILayout.Height(100)))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Second");
        }
    }
}
