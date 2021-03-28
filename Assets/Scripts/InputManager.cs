using System;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    public float dragHeight;
    public event Action OnResetSelection;
    public override void Awake()
    {
        base.Awake();
        BlackBoard.inputManager = _instance;
    }

    public Vector3 RayToPlanePosition(Ray ray) => InputTools.RayToPlanePosition(ray, dragHeight, PlaneOrientation.XZ);
    public Vector3 RayToPlanePosition(Ray ray, float height) => InputTools.RayToPlanePosition(ray, height, PlaneOrientation.XZ);
    public void ResetSelection()
    {
        OnResetSelection?.Invoke();
    }


}
