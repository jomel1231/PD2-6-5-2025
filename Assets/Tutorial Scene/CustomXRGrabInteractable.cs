using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomXRGrabInteractable : XRGrabInteractable
{
	[Header("Custom Attributes")]
	public bool isCable;
	public bool faulty;
	public CustomXRGrabInteractable otherEnd;

	protected override void Awake()
	{
		base.Awake();
		// Automatically determine if it's a cable based on the presence of otherEnd
		isCable = otherEnd != null;
	}

	// Check if the other end is currently grabbed or interacted with
	public bool IsOtherEndGrabbed
	{
		get
		{
			return isCable && otherEnd != null && otherEnd.isSelected;
		}
	}

	// You can expand this method as needed, for example, handle events upon grabbing
	protected override void OnSelectEntered(SelectEnterEventArgs args)
	{
		base.OnSelectEntered(args);
		// Custom logic when grabbed
		Debug.Log($"{gameObject.name} was grabbed.");
	}

	// Additional custom logic when releasing
	protected override void OnSelectExited(SelectExitEventArgs args)
	{
		base.OnSelectExited(args);
		// Custom logic when released
		Debug.Log($"{gameObject.name} was released.");
	}
}
