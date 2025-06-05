using UnityEngine;
using UnityEngine.Video;

public class VidController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false; // Important
            videoPlayer.Stop(); // 🔥 Freeze at first frame
        }
    }

    public void PlayCountdownVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }

    public void StopCountdownVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
    }
}
