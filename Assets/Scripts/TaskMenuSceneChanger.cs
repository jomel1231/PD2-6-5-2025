using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskMenuSceneManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
