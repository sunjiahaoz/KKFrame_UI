using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ToScene : MonoBehaviour
{
    public string[] strSceneName;

    void OnGUI()
    {
        for (int i = 0; i < strSceneName.Length; ++i)
        {
            if (GUILayout.Button(strSceneName[i], GUILayout.Width(100), GUILayout.Height(100)))
            {
                SceneManager.LoadScene(strSceneName[i]);
            }
        }
    }
}
