using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using System.Collections.Generic;

public class CustomHandMapper : MonoBehaviour
{
    public Handedness handedness = Handedness.Left;

    [System.Serializable]
    public class JointBonePair
    {
        public XRHandJointID jointID;
        public Transform boneTransform;
    }

    [Header("Custom Bone Mapping")]
    public List<JointBonePair> jointBoneMappings = new List<JointBonePair>();

    private XRHandSubsystem handSubsystem;

    void Start()
    {
        var xrManager = XRGeneralSettings.Instance?.Manager;
        if (xrManager != null)
        {
            handSubsystem = xrManager.activeLoader.GetLoadedSubsystem<XRHandSubsystem>();
        }

        if (handSubsystem == null)
        {
            Debug.LogError("XRHandSubsystem not found. Make sure XR Hands is installed and OpenXR is active.");
            enabled = false;
        }
    }

    void Update()
    {
        if (handSubsystem == null) return;

        XRHand hand = handedness == Handedness.Left ? handSubsystem.leftHand : handSubsystem.rightHand;
        if (!hand.isTracked) return;

        foreach (var pair in jointBoneMappings)
        {
            XRHandJoint joint = hand.GetJoint(pair.jointID);

            if (joint.TryGetPose(out Pose pose))
            {
                pair.boneTransform.position = pose.position;
                pair.boneTransform.rotation = pose.rotation;
            }
        }
    }
}
