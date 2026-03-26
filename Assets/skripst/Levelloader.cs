using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelOnKey : MonoBehaviour
{
    public string sceneName; // имя сцены
    public KeyCode key = KeyCode.E; // кнопка

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}