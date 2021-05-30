using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UiWindow : MonoBehaviour
{
    #region Events
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
    public void SelectWindow() => groupHandler.SelectWindow(this);
    public virtual void OnUpdate() { }
    public virtual void OnReset() { }
}
