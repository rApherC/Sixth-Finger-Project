using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerHandController : MonoBehaviour
{
    [Header("Controller Settings")]
    [Tooltip("Leave empty if using Unity 6 XR Origin - script will work without it")]
    public XRController xrController; // Optional in Unity 6
    public XRNode controllerNode = XRNode.RightHand; // or LeftHand

    [Header("Hand Animation")]
    public Animator handAnimator;

    [Header("Finger Bones - Main Hand")]
    public Transform[] thumbBones;
    public Transform[] indexBones;
    public Transform[] middleBones;
    public Transform[] ringBones;
    public Transform[] pinkyBones;

    [Header("Sixth Finger Bones")]
    public Transform[] sixthFingerBones;

    [Header("Animation Settings")]
    [Range(0f, 1f)]
    public float animationSpeed = 5f;

    [Header("Sixth Finger Control")]
    public SixthFingerMode sixthFingerMode = SixthFingerMode.FollowGrip;
    [Range(-30f, 30f)]
    public float sixthFingerOffset = 10f; // Degrees offset from middle finger

    // Private variables
    private float currentTrigger = 0f;
    private float currentGrip = 0f;
    private float currentSixthFinger = 0f;

    // Input device
    private InputDevice inputDevice;

    public enum SixthFingerMode
    {
        FollowGrip,        // Follows grip input
        FollowMiddle,      // Follows middle finger animation
        ThumbstickY,       // Controlled by thumbstick Y
        Independent        // Separate button control
    }

    void Start()
    {
        // Get the input device directly (works without XR Controller component)
        inputDevice = InputDevices.GetDeviceAtXRNode(controllerNode);

        // If XR Controller is assigned, use it (optional)
        if (xrController == null)
        {
            xrController = GetComponent<XRController>();
            if (xrController == null)
            {
                Debug.Log($"No XR Controller found - using direct input for {controllerNode}");
            }
        }

        Debug.Log($"Controller setup for {controllerNode}: Device valid = {inputDevice.isValid}");
    }

    void Update()
    {
        // Get controller input
        GetControllerInput();

        // Update hand animation
        UpdateHandAnimation();

        // Update sixth finger
        UpdateSixthFinger();
    }

    void GetControllerInput()
    {
        if (!inputDevice.isValid)
            inputDevice = InputDevices.GetDeviceAtXRNode(controllerNode);

        if (inputDevice.isValid)
        {
            // Get trigger value (0-1)
            inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
            currentTrigger = Mathf.Lerp(currentTrigger, triggerValue, Time.deltaTime * animationSpeed);

            // Get grip value (0-1)
            inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue);
            currentGrip = Mathf.Lerp(currentGrip, gripValue, Time.deltaTime * animationSpeed);

            // Debug
            Debug.Log($"Trigger: {currentTrigger:F2}, Grip: {currentGrip:F2}");
        }
    }

    void UpdateHandAnimation()
    {
        if (handAnimator != null)
        {
            // Set animator parameters (you'll create these in the Animator Controller)
            handAnimator.SetFloat("Trigger", currentTrigger);
            handAnimator.SetFloat("Grip", currentGrip);
            handAnimator.SetFloat("Pinch", currentTrigger); // For pinch gestures
        }
        else
        {
            // Direct bone manipulation if no animator
            AnimateFingersDirect();
        }
    }

    void AnimateFingersDirect()
    {
        // Simple direct bone rotation (backup if no animator)
        float indexCurl = currentTrigger * 90f; // Index finger follows trigger
        float otherFingersCurl = currentGrip * 90f; // Other fingers follow grip

        // Animate index finger
        AnimateFingerBones(indexBones, indexCurl);

        // Animate other fingers
        AnimateFingerBones(middleBones, otherFingersCurl);
        AnimateFingerBones(ringBones, otherFingersCurl);
        AnimateFingerBones(pinkyBones, otherFingersCurl);

        // Thumb is special - less curl
        AnimateFingerBones(thumbBones, (currentTrigger + currentGrip) * 0.5f * 45f);
    }

    void AnimateFingerBones(Transform[] bones, float curlAmount)
    {
        if (bones == null || bones.Length == 0) return;

        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                // Distribute curl across joints (more curl on later joints)
                float jointCurl = curlAmount * (i + 1) / bones.Length;
                Quaternion targetRotation = Quaternion.Euler(0, 0, jointCurl);
                bones[i].localRotation = Quaternion.Lerp(bones[i].localRotation, targetRotation, Time.deltaTime * animationSpeed);
            }
        }
    }

    void UpdateSixthFinger()
    {
        float sixthFingerTarget = 0f;

        switch (sixthFingerMode)
        {
            case SixthFingerMode.FollowGrip:
                sixthFingerTarget = currentGrip * 90f;
                break;

            case SixthFingerMode.FollowMiddle:
                sixthFingerTarget = currentGrip * 90f; // Same as middle finger
                break;

            case SixthFingerMode.ThumbstickY:
                if (inputDevice.isValid)
                {
                    inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbstick);
                    sixthFingerTarget = (thumbstick.y + 1) * 45f; // Convert -1,1 to 0,90
                }
                break;

            case SixthFingerMode.Independent:
                // Use a specific button for sixth finger
                if (inputDevice.isValid)
                {
                    inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressed);
                    sixthFingerTarget = buttonPressed ? 90f : 0f;
                }
                break;
        }

        // Add offset to avoid overlap
        sixthFingerTarget += sixthFingerOffset;

        // Animate sixth finger bones
        AnimateFingerBones(sixthFingerBones, sixthFingerTarget);

        currentSixthFinger = Mathf.Lerp(currentSixthFinger, sixthFingerTarget, Time.deltaTime * animationSpeed);
    }

    // Public methods for external control
    public void SetSixthFingerMode(SixthFingerMode newMode)
    {
        sixthFingerMode = newMode;
    }

    public void SetSixthFingerOffset(float offset)
    {
        sixthFingerOffset = offset;
    }
}