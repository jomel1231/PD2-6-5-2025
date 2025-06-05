using UnityEngine;
using System.Collections;

public class SimTrayInteraction : MonoBehaviour
{
    [SerializeField] private float ejectDistance = 0.01f; // Moves slightly out
    [SerializeField] private float ejectSpeed = 0.5f; // Speed of movement
    private bool isEjected = false;

    public void TriggerEject()
    {
        if (!isEjected)
        {
            StartCoroutine(EjectSimTray());
        }
    }

    private IEnumerator EjectSimTray()
    {
        isEjected = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + transform.right * ejectDistance; // Moves slightly to the right

        float elapsedTime = 0;
        while (elapsedTime < ejectSpeed)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / ejectSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; // Ensure it reaches the final position
    }
}
