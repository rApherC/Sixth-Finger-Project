using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class TriggerDetection : MonoBehaviour
{
    public string target;
    public UnityEvent<GameObject> OnEventTrigger;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == target){
            OnEventTrigger.Invoke(other.gameObject);
        }
    }
}
