using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // Assign VideoPlayer in Inspector
    public Slider progressBar;       // Assign Slider in Inspector
    public TextMeshProUGUI timeStamp; // Assign TextMeshProUGUI in Inspector

    private bool isDragging = false;

    void Start()
    {
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Prepare();

        progressBar.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        progressBar.maxValue = (float)videoPlayer.length;
    }

    void Update()
    {
        if (videoPlayer.isPrepared)
        {
            if (!isDragging)
            {
                progressBar.value = (float)videoPlayer.time;
            }

            UpdateTimeStamp();
        }
    }

    public void PlayVideo()
    {
        if (!videoPlayer.isPrepared)
            videoPlayer.Prepare();

        videoPlayer.Play();
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
    }

    public void OnSliderValueChanged(float value)
    {
        isDragging = true;
    }

    public void OnSliderDragEnd()
    {
        videoPlayer.time = progressBar.value;
        isDragging = false;
        if (!videoPlayer.isPlaying)
            videoPlayer.Play();
    }

    void UpdateTimeStamp()
    {
        int minutes = Mathf.FloorToInt((float)videoPlayer.time / 60);
        int seconds = Mathf.FloorToInt((float)videoPlayer.time % 60);
        int totalMinutes = Mathf.FloorToInt((float)videoPlayer.length / 60);
        int totalSeconds = Mathf.FloorToInt((float)videoPlayer.length % 60);

        timeStamp.text = $"{minutes:00}:{seconds:00} / {totalMinutes:00}:{totalSeconds:00}";
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Optional: Handle video completion
    }
}
