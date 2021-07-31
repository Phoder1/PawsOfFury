using Assets.StateMachine;
using CustomAttributes;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MenuUiManager : MonoSingleton<MenuUiManager>
{
    public MenuUiManager() : base(false)
    {
    }
    #region Serielized

    #region States
    [FoldoutGroup("States")]
    [SerializeField] MenuUiState incubator;
    [FoldoutGroup("States")]
    [SerializeField] MenuUiState upgrade;
    [FoldoutGroup("States")]
    [SerializeField] MenuUiState spellCraft;
    [FoldoutGroup("States")]
    [SerializeField] MenuUiState merge;
    [FoldoutGroup("States")]
    [SerializeField] MenuUiState minionBook;
    [FoldoutGroup("States")]
    [SerializeField] MenuUiState levelSelect;
    #endregion

    [FoldoutGroup("Transition")]
    [SerializeField] float transitionTime;
    [FoldoutGroup("Transition")]
    [SerializeField] AnimationCurve transitionCurve;
    [SerializeField] TextMeshProUGUI windowNameText;
    #endregion

    #region Events
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnUiLock;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnUiUnlock;
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
        stateMachine = new StateMachine<MenuUiState>(incubator);
    }
    #endregion

    #region State select
    public void SelectIncubatorWindow() => CurrentState = incubator;
    public void SelectUpgradeWindow() => CurrentState = upgrade;
    public void SelectSpellCraftWindow() => CurrentState = spellCraft;
    public void SelectMergeWindow() => CurrentState = merge;
    public void SelectMinionBookWindow() => CurrentState = minionBook;
    public void SelectLevelSelectWindow() => CurrentState = levelSelect;
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

        void OnComplete()
        {
            UiLocked = false;

            if (from != null && from != CurrentState)
                from.gameobject.SetActive(false);

            CurrentState.uiWindow.UiLocked = false;
        }
    }
}
[Serializable]
public class MenuUiState : State
{
    [Required]
    public UiWindow uiWindow;
     
    
    public GameObject gameobject => uiWindow.gameObject;
    public string Name => uiWindow.name.SplitCamelCase();
    protected override void OnEnable()
    {
        gameobject.SetActive(true);
    }
    protected override void OnDisable()
    {
        uiWindow.UiLocked = true;
    }
    protected override void OnUpdate() { }
}
