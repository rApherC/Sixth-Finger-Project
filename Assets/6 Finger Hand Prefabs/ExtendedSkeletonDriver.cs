using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;

public class XRHandExtendedSkeletonDriver : MonoBehaviour
{
    [Header("XR Hand Settings")]
    public XRHandSubsystem handSubsystem;
    public XRNode handNode = XRNode.RightHand; // or LeftHand

    [Header("Sixth Finger Bones")]
    public Transform sixthMetacarpal;
    public Transform sixthProximal;
    public Transform sixthIntermediate;
    public Transform sixthDistal;
    public Transform sixthTip;

    [Header("Source Finger (Reference for Copy)")]
    public Transform middleMetacarpal;
    public Transform middleProximal;
    public Transform middleIntermediate;
    public Transform middleDistal;
    public Transform middleTip;

    [Range(0f, 1f)]
    public float followStrength = 1f;

    [Range(-30f, 30f)]
    public float offsetAngle = 5f;

    void Start()
    {
        if (handSubsystem == null)
        {
            var subsystems = new List<XRHandSubsystem>();
            SubsystemManager.GetSubsystems(subsystems);
            if (subsystems.Count > 0)
                handSubsystem = subsystems[0];
        }
    }

    void LateUpdate()
    {
        if (handSubsystem == null || !handSubsystem.running)
            return;


        CopyRotationWithOffset(middleMetacarpal, sixthMetacarpal);
        CopyRotationWithOffset(middleProximal, sixthProximal);
        CopyRotationWithOffset(middleIntermediate, sixthIntermediate);
        CopyRotationWithOffset(middleDistal, sixthDistal);
        CopyRotationWithOffset(middleTip, sixthTip);
    }

    void CopyRotationWithOffset(Transform source, Transform target)
    {
        if (source == null || target == null)
            return;

        Quaternion sourceRot = source.localRotation;
        Quaternion offset = Quaternion.Euler(0, offsetAngle, 0);
        Quaternion targetRot = sourceRot * offset;
        target.localRotation = Quaternion.Lerp(target.localRotation, targetRot, followStrength);
    }
}
