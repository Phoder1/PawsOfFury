using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoolPassthrough : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<bool> OnTrueEvent;
    [SerializeField]
    private UnityEvent<bool> OnFalseEvent;

    public void Trigger(bool value)
    {
        if (value)
            OnTrueEvent?.Invoke(true);
        else
            OnFalseEvent?.Invoke(false);
    }
}
