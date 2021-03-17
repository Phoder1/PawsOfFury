using Assets.StateMachine;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DragAndDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private enum PressState { DefaultState, Pressed, Dragging }
    private PressState pressState;
    [SerializeField] TextMeshProUGUI text;

    StateMachine<ButtonState> stateMachine;
    protected Camera mainCam;
    protected LevelManager levelManager;
    protected InputManager inputManager;
    protected abstract ButtonState GetDefaultState();
    protected abstract ButtonState GetPressedState();
    protected abstract ButtonState GetDraggedState();
    protected abstract string ButtonText();
    protected virtual void Start()
    {
        mainCam = Camera.main;
        levelManager = LevelManager._instance;
        inputManager = InputManager._instance;
        text.text = ButtonText();
        stateMachine = new StateMachine<ButtonState>(GetDefaultState());
    }
    private void Update() => stateMachine.Update();
    public void OnPointerDown(PointerEventData eventData)
    {
        if (pressState == PressState.DefaultState)
        {
            pressState = PressState.Pressed;
            stateMachine.State = GetPressedState();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (pressState == PressState.Pressed)
        {
            pressState = PressState.Dragging;
            stateMachine.State = GetDraggedState();
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (pressState == PressState.Dragging)
            Drop();
        pressState = PressState.DefaultState;
        stateMachine.State = GetDefaultState();
    }
    protected abstract void Drop();
    protected abstract class ButtonState : State
    {

        protected DragAndDrop button;

        protected ButtonState(DragAndDrop button)
        {
            this.button = button ?? throw new ArgumentNullException(nameof(button));
        }
    }
    class DefaultState : ButtonState
    {
        public DefaultState(DragAndDrop button) : base(button) { }
    }
}
