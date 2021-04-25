using Assets.StateMachine;
using CustomAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;

public class MenuUiManager : MonoSingleton<MenuUiManager>
{
    [SerializeField] MenuUiState mainUiState;

    StateMachine<MenuUiState> stateMachine;
    bool inputLocked = true;
    private void Start()
    {
        //stateMachine = new StateMachine<MenuUiState>(mainUiState);
    }
}
public interface IUiWindow
{
    void OnReset();
    void OnUpdate();
    //void StartInTransition(Event callback = null);
    //void StartOutTransition(Event callback = null);
}
[Serializable]
public class MenuUiState : State
{
    [SerializeField] GameObject uiWindow;
    [LocalComponent(parentObject: "uiWindow")]
    public UiWindow windowComponent;
    protected override void OnEnable() => uiWindow.SetActive(true);
    protected override void OnDisable() => uiWindow.SetActive(false);
    protected override void OnUpdate() { }
}
