using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    [SerializeField] float rotationOffset;
    // Start is called before the first frame update
    void Awake()
    {
        RotateToCamera();
        GameManager.instance.OnNewScene += RotateToCamera;
    }

    public void RotateToCamera()
    {
        transform.rotation = Quaternion.Euler(GameManager.MainCam.transform.rotation.eulerAngles.x + rotationOffset, 0, 0);
    }
}
