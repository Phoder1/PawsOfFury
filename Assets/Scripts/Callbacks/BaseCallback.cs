using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseCallback : MonoBehaviour
{
    [SerializeField]
    private UnityEvent CallbackTrigger;

    protected void CallEvent()
    {
        CallbackTrigger?.Invoke();
    }
}
