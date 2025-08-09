using UnityEngine;

public class SixthFingerController : MonoBehaviour
{
    [Header("Middle Finger Bones (Source)")]
    public Transform middleMetacarpal; 
    public Transform middleProximal;   
    public Transform middleIntermediate; 
    public Transform middleDistal;     
    public Transform middleTip;       

    [Header("Sixth Finger Bones (Target)")]
    public Transform sixthMetacarpal;
    public Transform sixthProximal;
    public Transform sixthIntermediate;
    public Transform sixthDistal;
    public Transform sixthTip;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float followStrength = 1f; 

    [Range(-30f, 30f)]
    public float offsetAngle = 5f;

    void LateUpdate()
    {

        Debug.Log("Middle finger exists: " + (middleTip != null));
        Debug.Log("Sixth finger exists: " + (sixthTip != null));

  
        if (middleTip != null)
            middleTip.Rotate(0, 0, 10 * Time.deltaTime);
        if (sixthTip != null)
            sixthTip.Rotate(0, 0, 10 * Time.deltaTime);

        // Copy rotations with optional offset
        CopyRotationWithOffset(middleMetacarpal, sixthMetacarpal);
        CopyRotationWithOffset(middleProximal, sixthProximal);
        CopyRotationWithOffset(middleIntermediate, sixthIntermediate);
        CopyRotationWithOffset(middleDistal, sixthDistal);
        CopyRotationWithOffset(middleTip, sixthTip);
    }

    void CopyRotationWithOffset(Transform source, Transform target)
    {
        if (source == null || target == null) return;


        Quaternion sourceRotation = source.rotation;

        Quaternion offset = Quaternion.Euler(0, offsetAngle, 0);
        Quaternion targetRotation = sourceRotation * offset;

        target.rotation = Quaternion.Lerp(target.rotation, targetRotation, followStrength);
    }
}