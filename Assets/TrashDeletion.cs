using UnityEngine;

public class TrashDeletion : MonoBehaviour
{
    void Start()
    {
        GetComponent<TriggerDetection>().OnEventTrigger.AddListener(PassedThrough);
    }
    public void PassedThrough(GameObject block){
        block.SetActive(false);
    }
}
