using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventToggle : MonoBehaviour
{
    [SerializeField]
    private bool _toggleValue;

    [SerializeField]
    private UnityEvent OnTrueEvent;
    [SerializeField]
    private UnityEvent OnFalseEvent;

    private void Start()
    {
        
    }
    public void ToggleAndTrigger()
    {
        Toggle();
        Trigger();
    }
    public void SetStateAndTrigger(bool value)
    {
        SetState(value);
        Trigger();
    }
    public void Toggle()
    {
        _toggleValue = !_toggleValue;
    }
    public void Trigger()
    {
        if (_toggleValue)
            OnTrueEvent?.Invoke();
        else
            OnFalseEvent?.Invoke();
    }
    public void SetState(bool value) => _toggleValue = value;    
}
