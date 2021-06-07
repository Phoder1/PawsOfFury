using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UiWindow : MonoBehaviour
{
    #region Events
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnTransitionIn_Start;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnTransitionIn_End;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnSetActiveEvent;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnSetInactiveEvent;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnTransitionOut_Start;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnTransitionOut_End;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnUiLock;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnUiUnlock;

    [HideInInspector]
    public WindowGroupHandler groupHandler;
    #endregion
    private bool _uiLocked;
    public virtual bool UiLocked
    {
        get => _uiLocked;
        set
            => Misc.Setter(ref _uiLocked, value, () =>
            {
                if (_uiLocked)
                    OnUiLock?.Invoke();
                else
                    OnUiUnlock?.Invoke();
            });
    }
    public void SetActive(bool state)
    {

        (state ? OnSetActiveEvent : OnSetInactiveEvent)?.Invoke();
        UiLocked = !state;
        OnSetActive();
    }
    public virtual void OnSetActive() { }
    public void SelectWindow() => groupHandler.SelectWindow(this);
    public virtual void OnUpdate() { }
    public virtual void OnReset() { }
    public void TransitionInStart() => OnTransitionIn_Start?.Invoke();
    public void TransitionInEnd()
    {
        OnTransitionIn_End?.Invoke();
        SetActive(true);
    }
    public void TransitionOutStart()
    {
        OnTransitionOut_Start?.Invoke();
        SetActive(false);
    }
    public void TransitionOutEnd() => OnTransitionOut_End?.Invoke();
}
