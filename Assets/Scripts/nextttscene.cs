using UnityEngine;
using UnityEngine.SceneManagement; // Import Scene Management

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad; // Set this in the Inspector

    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad)) // Check if scene name is set
        {
            SceneManager.LoadScene(sceneToLoad); // Load the selected scene
        }
        else
        {
            Debug.LogError("Scene name is not set! Assign it in the Inspector.");
        }
    }
}
