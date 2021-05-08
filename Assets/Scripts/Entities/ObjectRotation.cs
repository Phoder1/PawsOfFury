using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    [SerializeField] float rotationOffset;
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x + rotationOffset, 0, 0);
    }

}
