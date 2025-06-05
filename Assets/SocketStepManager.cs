using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketStepManager : MonoBehaviour
{
    [Header("Projector")]
    public ProjectorTaskManager projectorTaskManager;

    [Header("Assessment Result")]
    public AssessmentResultMonitor resultMonitor;

    [Header("External Managers")]
    public PentalobeGroupAssembly pentalobeGroupManager;
    public TriPointGroupAssembly triPointGroupManager;
    public DashlineGroupManagerAssembly dashlineGroupManagerAssembly;
    public Animator screenAnimator;

    [Header("Step 1: Adhesive")]
    public List<XRSocketInteractor> adhesiveSockets = new List<XRSocketInteractor>();
    public int step1ProjectorIndex = 0;

    [Header("Step 2: Battery")]
    public XRSocketInteractor batterySocket;
    public GameObject batteryCableObject;
    public int step2ProjectorIndex = 1;

    [Header("Step 3: Battery Cable Animator")]
    public Animator cableAnimatorStep3;
    public int step3ProjectorIndex = 2;

    [Header("Step 4: Screen Cable Animators")]
    public Animator cableAnimator1;
    public Animator cableAnimator2;
    public List<XRSocketInteractor> screenCableSockets = new List<XRSocketInteractor>();
    public int step4ProjectorIndex = 3;

    [Header("Step 5: Metal Shield Sockets")]
    public List<XRSocketInteractor> shieldSockets = new List<XRSocketInteractor>();
    public int step5ProjectorIndex = 4;

    [Header("Step 6: Check Tri-point Screws")]
    public GameObject objectToEnableOnStep6;
    public int step6ProjectorIndex = 5;

    [Header("Step 7: Dashline Guideline")]
    public int step7ProjectorIndex = 6;

    [Header("Step 8: Screen Animator")]
    public int step8ProjectorIndex = 7;

    [Header("Step 9: Check Pentalobe Screws")]
    public int step9ProjectorIndex = 8;

    [Header("Metal Shield Parts (Grab Control)")]
    public List<XRGrabInteractable> metalShieldParts = new List<XRGrabInteractable>();

    private bool[] stepCompleted = new bool[9];

    void Start()
    {
        if (batterySocket != null)
        {
            batterySocket.selectEntered.AddListener(_ => OnBatterySocketed());
            batterySocket.selectExited.AddListener(_ => OnBatteryRemoved());
        }

        if (batteryCableObject != null)
            batteryCableObject.SetActive(false);

        if (objectToEnableOnStep6 != null)
            objectToEnableOnStep6.SetActive(false);

        UpdateMetalShieldGrabState(); // Initialize grab state on start
    }

    void Update()
    {
        CheckStep1();
        CheckStep2();
        CheckStep3();
        CheckStep4();
        CheckStep5();
        CheckStep6();
        CheckStep7();
        CheckStep8();
        CheckStep9();
    }

    void CheckStep1()
    {
        if (stepCompleted[0]) return;

        int filled = 0;
        foreach (var socket in adhesiveSockets)
            if (socket.hasSelection) filled++;

        if (filled >= adhesiveSockets.Count)
            CompleteStep(0, step1ProjectorIndex);
    }

    void CheckStep2()
    {
        if (stepCompleted[1] || !CheckPrerequisite(1)) return;

        if (batterySocket.hasSelection)
            CompleteStep(1, step2ProjectorIndex);
    }

    void CheckStep3()
    {
        if (stepCompleted[2] || !CheckPrerequisite(2)) return;

        foreach (var socket in shieldSockets)
            if (socket != null && socket.hasSelection)
                return;

        if (cableAnimatorStep3 != null && cableAnimatorStep3.enabled)
            CompleteStep(2, step3ProjectorIndex);
    }

    void CheckStep4()
    {
        if (stepCompleted[3] || !CheckPrerequisite(3)) return;

        foreach (var socket in screenCableSockets)
            if (socket != null && socket.hasSelection)
                return;

        if (cableAnimator1 != null && cableAnimator2 != null && cableAnimator1.enabled && cableAnimator2.enabled)
            CompleteStep(3, step4ProjectorIndex);
    }

    void CheckStep5()
    {
        if (stepCompleted[4] || !CheckPrerequisite(4)) return;

        int filled = 0;
        foreach (var socket in shieldSockets)
            if (socket.hasSelection) filled++;

        if (filled >= shieldSockets.Count)
            CompleteStep(4, step5ProjectorIndex);
    }

    void CheckStep6()
    {
        if (triPointGroupManager == null) return;

        bool shouldBeCompleted = triPointGroupManager.IsGroupDone();
        if (stepCompleted[5] != shouldBeCompleted)
        {
            stepCompleted[5] = shouldBeCompleted;

            if (shouldBeCompleted)
            {
                CompleteStep(5, step6ProjectorIndex);
                if (objectToEnableOnStep6 != null)
                    objectToEnableOnStep6.SetActive(true);
            }

            UpdateMetalShieldGrabState(); // ✅ Update shield grab ability
        }
    }

    void CheckStep7()
    {
        if (stepCompleted[6]) return;

        // Require steps 0–6 to be completed before allowing Step 7
        for (int i = 0; i <= 5; i++)
        {
            if (!stepCompleted[i]) return;
        }

        if (dashlineGroupManagerAssembly != null && dashlineGroupManagerAssembly.IsDashlineTaskDone())
        {
            CompleteStep(6, step7ProjectorIndex);
        }
    }

    void CheckStep8()
    {
        if (!stepCompleted[7] && screenAnimator != null && screenAnimator.enabled)
            CompleteStep(7, step8ProjectorIndex);
    }

    void CheckStep9()
    {
        if (stepCompleted[8] || !CheckPrerequisite(8)) return;

        if (pentalobeGroupManager != null && pentalobeGroupManager.IsGroupDone())
            CompleteStep(8, step9ProjectorIndex);
    }

    void OnBatterySocketed()
    {
        if (batteryCableObject != null)
            batteryCableObject.SetActive(true);
    }

    void OnBatteryRemoved()
    {
        if (batteryCableObject != null)
            batteryCableObject.SetActive(false);
    }

    void CompleteStep(int stepIndex, int projectorIndex)
    {
        if (stepIndex < 0 || stepIndex >= stepCompleted.Length) return;
        if (stepCompleted[stepIndex]) return;

        stepCompleted[stepIndex] = true;
        projectorTaskManager?.MarkTaskComplete(projectorIndex);

        if (stepIndex == 8 && resultMonitor != null)
        {
            resultMonitor.ForceImmediateResult();
        }
    }

    void UpdateMetalShieldGrabState()
    {
        bool allowGrab = !stepCompleted[5]; // Only allow grab if Step 6 is not complete

        foreach (var part in metalShieldParts)
        {
            if (part != null)
                part.enabled = allowGrab;
        }
    }

    public bool IsStepDone(int index)
    {
        return index >= 0 && index < stepCompleted.Length && stepCompleted[index];
    }

    public void MarkStep7FromGlue()
    {
        if (!stepCompleted[6])
        {
            stepCompleted[6] = true;
            projectorTaskManager?.MarkTaskComplete(step7ProjectorIndex);
        }
    }

    bool CheckPrerequisite(int stepIndex)
    {
        if (stepIndex == 7) return true;
        if (stepIndex == 8) return stepCompleted[7];
        return stepIndex == 0 || stepCompleted[stepIndex - 1];
    }
}
