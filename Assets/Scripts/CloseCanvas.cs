using UnityEngine;

public class CloseCanvas : MonoBehaviour
{
    public GameObject canvas;

    public void Close()
    {
        canvas.SetActive(false);
    }
}
