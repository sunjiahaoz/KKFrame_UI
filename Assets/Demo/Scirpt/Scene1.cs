using UnityEngine;
using System.Collections;
using KK.Frame.UI;

public class Scene1 : MonoBehaviour {
    void OnGUI()
    {
        if (GUILayout.Button("Click", GUILayout.Width(100), GUILayout.Height(100)))
        {
            UIController.ShowUI("ID_FrameTest2");            
        }
        if (GUILayout.Button("ToSecond", GUILayout.Width(100), GUILayout.Height(100)))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Second");
        }
    }	
}
