using UnityEngine;
using System.Collections.Generic;

public class CableCoverUpdater : MonoBehaviour
{
    [Header("Spheres (Points): From Start to End")]
    public List<Transform> points;

    [Header("Cylinders (Connectors): Covers between Points")]
    public List<Transform> connectors;

    [Header("Cylinder Thickness")]
    public float thickness = 0.02f;

    void LateUpdate()
    {
        for (int i = 0; i < connectors.Count; i++)
        {
            Transform startPoint = points[i];
            Transform endPoint = points[i + 1];
            Transform connector = connectors[i];

            UpdateConnector(connector, startPoint.position, endPoint.position);
        }
    }

    void UpdateConnector(Transform connector, Vector3 from, Vector3 to)
    {
        connector.position = (from + to) * 0.5f;

        Vector3 direction = to - from;
        float length = direction.magnitude;

        if (length > Mathf.Epsilon)
            connector.rotation = Quaternion.LookRotation(direction);

        connector.localScale = new Vector3(thickness, thickness, length);
    }
}
