using System;
using UnityEngine;
using Assets.StateMachine;

public enum PressState { DefaultState, Pressed, Dragging }
public class InputManager : MonoSingleton<InputManager>
{
    public float dragHeight;
    public event Action OnResetSelection;
    StateMachine<ButtonsState> stateMachine;
    public ButtonsState dragState;
    private PressState pressState;
    public PressState PressState { 
        get => pressState;
        set { 
            if(pressState != value)
            {
                pressState = value;
                switch (pressState)
                {
                    case PressState.DefaultState:
                    case PressState.Pressed:
                    default:
                        stateMachine.State = null;
                        break;
                    case PressState.Dragging:
                        stateMachine.State = dragState;
                        break;
                }
            }
        }
    }

    public override void OnAwake() => IngameBlackBoard.inputManager = _instance;
    private void Start()
    {
        stateMachine = new StateMachine<ButtonsState>(null);
    }
    private void Update() => stateMachine.Update();
    public void StartDragging() => stateMachine.State = dragState;

    public Vector3 RayToPlanePosition(Ray ray) => InputTools.RayToPlanePosition(ray, dragHeight, PlaneOrientation.XZ);
    public Vector3 RayToPlanePosition(Ray ray, float height) => InputTools.RayToPlanePosition(ray, height, PlaneOrientation.XZ);
    public void ResetSelection()
    {
        OnResetSelection?.Invoke();
    }

    public abstract class ButtonsState : State
    {

        protected DragAndDrop button;

        protected ButtonsState(DragAndDrop button)
        {
            this.button = button ?? throw new ArgumentNullException(nameof(button));
        }
    }
}
