using UnityEngine;
using UnityEngine.Video;

public class AssemblyVideoTimerCheck : MonoBehaviour
{
    [Header("Timer Video")]
    public VideoPlayer timerVideo; // Visual timer (optional)

    private bool hasTriggered = false;

    void Start()
    {
        if (timerVideo != null)
            timerVideo.Stop(); // ✅ Make sure timer video is STOPPED at start
    }

    public void PlayTimer()
    {
        if (timerVideo != null)
        {
            timerVideo.Play(); // ✅ Only manually call this from Start Button
            Debug.Log("▶️ Timer video started manually.");
        }
    }

    public void OnProcessComplete()
    {
        if (hasTriggered) return;

        hasTriggered = true;

        if (timerVideo != null && timerVideo.isPlaying)
            timerVideo.Pause(); // Pause the video when complete

        Debug.Log($"✅ Repair process completed successfully.");
    }

    public void OnProcessFail()
    {
        if (hasTriggered) return;

        hasTriggered = true;

        if (timerVideo != null && timerVideo.isPlaying)
            timerVideo.Pause(); // Pause the video on fail

        Debug.Log($"❌ Repair process failed (time expired or incomplete).");
    }
}
    