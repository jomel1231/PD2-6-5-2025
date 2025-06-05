using UnityEngine;

public class ForceHideOnAwake : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false); // 🔒 force hidden at launch
    }
}
