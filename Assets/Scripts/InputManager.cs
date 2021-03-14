using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    Camera mainCam;
    public float dragHeight;
    public event Action OnChangedSelection;
    private void Start()
    {
        mainCam = Camera.main;
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);
        }
    }
    public Vector3 RayToPlanePosition(Ray ray)
    {

        float X = ray.origin.x + ray.direction.x * (dragHeight - ray.origin.y) / ray.direction.y;
        float Z = ray.origin.z + ray.direction.z * (dragHeight - ray.origin.y) / ray.direction.y;
        return new Vector3(X, dragHeight, Z);

    }
    public void ChangeSelection()
    {
        OnChangedSelection?.Invoke();
    }
}
