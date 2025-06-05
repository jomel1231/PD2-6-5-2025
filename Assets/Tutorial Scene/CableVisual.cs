using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CableVisual : MonoBehaviour
{
    public Transform plugStart;
    public Transform plugEnd;
    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
    }

    void Update()
    {
        line.SetPosition(0, plugStart.position);
        line.SetPosition(1, plugEnd.position);
    }
}
