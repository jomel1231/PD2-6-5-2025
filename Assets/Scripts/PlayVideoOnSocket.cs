using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayVideoOnSocket : MonoBehaviour
{
    public VideoPlayer videoPlayer;     // 🎥 Drag your Video Player here
    public GameObject videoCanvasUI;    // 🎬 Assign your UI canvas (e.g., "iPhone opening")
    public string requiredTag = "NewScreen";

    private XRSocketInteractor socket;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnObjectSocketed);

        // Optional: Hide UI at start (safety check)
        if (videoCanvasUI != null)
            videoCanvasUI.SetActive(false);
    }

    private void OnObjectSocketed(SelectEnterEventArgs args)
    {
        GameObject obj = args.interactableObject.transform.gameObject;

        if (obj.CompareTag(requiredTag))
        {
            if (videoCanvasUI != null)
                videoCanvasUI.SetActive(true); // ✅ Show the video screen UI

            if (videoPlayer != null && !videoPlayer.isPlaying)
            {
                videoPlayer.Play(); // ✅ Start the boot video
                Debug.Log("📱 Video started after new screen inserted!");
            }
        }
    }

    private void OnDestroy()
    {
        if (socket != null)
            socket.selectEntered.RemoveListener(OnObjectSocketed);
    }
}
