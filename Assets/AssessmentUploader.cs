using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AssessmentUploader : MonoBehaviour
{
    [System.Serializable]
    public class AssessmentData
    {
        public string assessmentType;     // "Battery" or "Bootloop"
        public string timeUsed;           // e.g., "00:01"
        public string taskCompleted;      // e.g., "P1: ..., P2: ..."
        public string finalScore;         // e.g., "18/22"
        public string result;             // "PASSED" or "FAILED"
    }

    [Header("Google Apps Script Web App URL")]
    public string webAppUrl;

    public void UploadResult(string timeUsed, string taskCompleted, string finalScore, string result)
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string assessmentType = sceneName.Contains("Battery") ? "Battery" : "Bootloop";

        AssessmentData data = new AssessmentData
        {
            assessmentType = assessmentType,
            timeUsed = timeUsed,
            taskCompleted = taskCompleted,
            finalScore = finalScore,
            result = result
        };

        string jsonData = JsonUtility.ToJson(data);
        StartCoroutine(SendToGoogleSheet(jsonData));
        Debug.Log($"[UPLOAD RESULT] Type: {assessmentType}, Time: {timeUsed}, TaskCompleted: {taskCompleted}, Score: {finalScore}, Result: {result}");
    }

    private IEnumerator SendToGoogleSheet(string json)
    {
        using UnityWebRequest request = new UnityWebRequest(webAppUrl, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload successful: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Upload failed: " + request.error);
        }
    }
}
