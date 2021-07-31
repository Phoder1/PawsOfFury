using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    [SerializeField] float rotationOffset = 0;

    private void OnEnable()
    {
        RotateToCamera();
        GameManager.instance.OnNewScene += RotateToCamera;
    }
    private void OnDisable()
    {
        GameManager.instance.OnNewScene -= RotateToCamera;
    }

    public void RotateToCamera()
    {
        var camTransform = GameManager.MainCam?.transform;
        if (camTransform != null)
            transform.rotation = Quaternion.Euler(camTransform.rotation.eulerAngles.x + rotationOffset, 0, 0);
    }
}
