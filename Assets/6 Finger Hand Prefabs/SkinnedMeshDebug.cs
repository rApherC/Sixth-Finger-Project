using UnityEngine;

public class SkinnedMeshDebugger : MonoBehaviour
{
    void Start()
    {
        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
        if (smr == null)
        {
            Debug.LogError("SkinnedMeshRenderer not found on GameObject: " + gameObject.name);
            return;
        }

        Debug.Log("Total bones assigned: " + smr.bones.Length);
        for (int i = 0; i < smr.bones.Length; i++)
        {
            Transform bone = smr.bones[i];
            Debug.Log($"Bone {i}: {bone.name}");
        }
    }
}
