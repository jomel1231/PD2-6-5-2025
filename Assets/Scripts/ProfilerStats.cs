using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using System.Text;
 
public class ProfilerStats : MonoBehaviour
{
 
    ProfilerRecorder triangleRecorder;
    ProfilerRecorder drawCallsRecorder;
    ProfilerRecorder verticesRecorder;
    public TMPro.TextMeshProUGUI statOverlay;
 
    private int framesCount;
    private float framesTime, lastFPS;
 
    // Start is called  before the first frame update
    void Start()
    {
        if(statOverlay == null)
            statOverlay = GetComponent<TMPro.TextMeshProUGUI>();      
    }
    void OnEnable()
    {
        triangleRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
        drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        verticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
    }
 
    void OnDisable()
    {
        triangleRecorder.Dispose();
        drawCallsRecorder.Dispose();
        verticesRecorder.Dispose();
    }
 
 
    // Update is called once per frame
    void Update()
    {
        var sb = new StringBuilder(500);
 
        framesCount++;
        framesTime += Time.unscaledDeltaTime;
        if(framesTime > 0.5f)
        {
            float fps = framesCount/framesTime;
            lastFPS = fps;
            framesCount = 0;
            framesTime = 0;
        }
        sb.AppendLine($"FPS: {lastFPS}");
        sb.AppendLine($"Verts: {verticesRecorder.LastValue/1000}k");
        sb.AppendLine($"Tris: {triangleRecorder.LastValue/1000}k");
        sb.AppendLine($"DrawCalls: {drawCallsRecorder.LastValue}");
 
        statOverlay.text = sb.ToString();
    }
   
}