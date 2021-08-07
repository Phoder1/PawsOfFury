using Assets.StateMachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WindowGroupHandler : MonoBehaviour
{
    #region Serielized

    #region States

    [FoldoutGroup("States"), ValueDropdown("TreeViewOfInts")]
    [SerializeField] private int _defaultStateNumber;
    [FoldoutGroup("States"), ListDrawerSettings(Expanded = true)]
    [SerializeField] private List<MenuUiState> states;
    #endregion
    [FoldoutGroup("Transition")]
    [SerializeField] float transitionTime;
    [FoldoutGroup("Transition")]
    [SerializeField] AnimationCurve transitionCurve;
    [SerializeField] TextMeshProUGUI windowNameText;
    #endregion
    private IEnumerable TreeViewOfInts => states.ConvertAll((x) => new ValueDropdownItem(x.Name, states.FindIndex((y) => y == x)));
    #region Events
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnUiLock;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnUiUnlock;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnTransitionStart;
    #endregion

    #region State
    private Tween transitionTween;
    private StateMachine<MenuUiState> stateMachine;
    public MenuUiState CurrentState
    {
        get => stateMachine.State;
        set
        {
            if (CurrentState == value)
                return;


            var lastState = CurrentState;

            stateMachine.State = value;

            OnStateChange(lastState);
        }
    }
    private bool _uiLocked;
    public bool UiLocked
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
    #endregion

    #region Unity callbacks
    private void Start()
    {
        states.ForEach((x) => x.uiWindow.groupHandler = this);
        stateMachine = new StateMachine<MenuUiState>(states[_defaultStateNumber]);
        transform.localPosition -= CurrentState.uiWindow.transform.localPosition;
        windowNameText.text = CurrentState.uiWindow.name;
        CurrentState.uiWindow.SetActive(true);
    }
    #endregion

    #region State select
    public void SelectWindow(UiWindow window) => CurrentState = states.Find((x) => x.uiWindow == window);
    #endregion
    private void OnStateChange(MenuUiState from)
    {
        UiLocked = true;

        if (transitionTween != null)
        {
            transitionTween.onComplete?.Invoke();
            transitionTween.Kill();
        }

        windowNameText.text = CurrentState.uiWindow.name;

        Vector3 uiStatePos = CurrentState.uiWindow.transform.localPosition;
        transitionTween = transform.DOLocalMove(-uiStatePos, transitionTime).SetEase(transitionCurve);

        transitionTween.OnComplete(OnComplete);

        from.uiWindow.TransitionOutStart();
        CurrentState.uiWindow.TransitionInStart();

        OnTransitionStart?.Invoke();

        void OnComplete()
        {
            UiLocked = false;

            from.uiWindow.TransitionOutEnd();
            CurrentState.uiWindow.TransitionInEnd();

            if (from != null && from != CurrentState)
                from.gameobject.SetActive(false);

            CurrentState.uiWindow.UiLocked = false;

        }
    }
}
