using System.Collections;
using UnityEngine;

public class PhoneUIStateManager : MonoBehaviour
{
    public enum PhoneState { Frozen, Booting, LockScreen, AppScreen, Off }

    [Header("UI Screens")]
    public GameObject unresponsiveUI;
    public GameObject appleLogoUI;
    public GameObject lockScreenUI;
    public GameObject appScreenUI;

    [Header("Button Objects")]
    public GameObject volumeUpButton;
    public GameObject volumeDownButton;
    public GameObject powerButton;

    [Header("Highlight Materials")]
    public Material highlightMaterial;
    public Material defaultMaterial;

    private PhoneState currentState = PhoneState.Frozen;
    public PhoneState GetCurrentState() => currentState;

    private int currentStep = 0; // 0 = Up, 1 = Down, 2 = Power

    private Renderer upRenderer, downRenderer, powerRenderer;
    private Material originalUpMaterial, originalDownMaterial, originalPowerMaterial;
    private Renderer previouslyHighlightedRenderer;

    void Start()
    {
        upRenderer = volumeUpButton.GetComponent<Renderer>();
        downRenderer = volumeDownButton.GetComponent<Renderer>();
        powerRenderer = powerButton.GetComponent<Renderer>();

        originalUpMaterial = upRenderer.material;
        originalDownMaterial = downRenderer.material;
        originalPowerMaterial = powerRenderer.material;

        ShowState(PhoneState.Frozen);
        SetHighlight(0);
    }

    public void PressVolumeUp()
    {
        if (currentStep == 0)
        {
            currentStep = 1;
            SetHighlight(1);
        }
    }

    public void PressVolumeDown()
    {
        if (currentStep == 1)
        {
            currentStep = 2;
            SetHighlight(2);
        }
    }

    public void PressPowerButton()
    {
        Debug.Log("👉 Power button pressed | State: " + currentState);

        // Power ON from Off
        if (currentState == PhoneState.Off)
        {
            Debug.Log("🔋 Power ON → LockScreen");
            ShowState(PhoneState.LockScreen);
            return;
        }

        // Turn OFF if already on
        if (currentState == PhoneState.LockScreen || currentState == PhoneState.AppScreen)
        {
            Debug.Log("🔌 Power OFF from: " + currentState);
            ShowState(PhoneState.Off);
            return;
        }

        // First-time boot sequence
        if (currentStep == 2)
        {
            currentStep = 3;
            SetHighlight(-1);
            StartCoroutine(BootSequence());
        }
    }

    private IEnumerator BootSequence()
    {
        ShowState(PhoneState.Booting);
        yield return new WaitForSeconds(3f);
        ShowState(PhoneState.LockScreen);
    }

    public void SwipeToUnlock()
    {
        if (currentState != PhoneState.LockScreen) return;

        Debug.Log("📲 Unlock triggered by click!");
        ShowState(PhoneState.AppScreen);
    }

    public void ForceStateToApp()
    {
        currentState = PhoneState.AppScreen;
    }

    private void ShowState(PhoneState state)
    {
        currentState = state;

        // Turn off all UI first to ensure clean state
        unresponsiveUI.SetActive(false);
        appleLogoUI.SetActive(false);
        lockScreenUI.SetActive(false);
        appScreenUI.SetActive(false);

        // Debug print to ensure visibility
        Debug.Log($"📲 Showing UI State: {state}");

        switch (state)
        {
            case PhoneState.Frozen:
                unresponsiveUI.SetActive(true);
                break;
            case PhoneState.Booting:
                appleLogoUI.SetActive(true);
                break;
            case PhoneState.LockScreen:
                Debug.Log("🔐 lockScreenUI is: " + (lockScreenUI != null ? "Assigned" : "NULL"));
                lockScreenUI.SetActive(true);
                break;
            case PhoneState.AppScreen:
                appScreenUI.SetActive(true);
                break;
            case PhoneState.Off:
                Debug.Log("🛑 Turning OFF all UIs!");
                // Already disabled above
                break;
        }
    }



    private void SetActiveAllChildren(GameObject parent, bool active)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(active);
        }
        parent.SetActive(active);
    }
    private void SetHighlight(int step)
    {
        if (previouslyHighlightedRenderer != null)
        {
            if (previouslyHighlightedRenderer == upRenderer)
                upRenderer.material = originalUpMaterial;
            else if (previouslyHighlightedRenderer == downRenderer)
                downRenderer.material = originalDownMaterial;
            else if (previouslyHighlightedRenderer == powerRenderer)
                powerRenderer.material = originalPowerMaterial;

            previouslyHighlightedRenderer = null;
        }

        if (step == 0)
        {
            upRenderer.material = highlightMaterial;
            previouslyHighlightedRenderer = upRenderer;
        }
        else if (step == 1)
        {
            downRenderer.material = highlightMaterial;
            previouslyHighlightedRenderer = downRenderer;
        }
        else if (step == 2)
        {
            powerRenderer.material = highlightMaterial;
            previouslyHighlightedRenderer = powerRenderer;
        }
    }
}