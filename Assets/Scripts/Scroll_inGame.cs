using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Scroll_inGame : MonoBehaviour
{
    Vector2 startTouch;
    Vector2 moveTouch;
    private bool isTouching;
    private float Z;
    [SerializeField]
    private float moveDistanceToScroll;
    [SerializeField]
    float minZ;
    [SerializeField]
    float maxZ;


    Rigidbody rb;



    [SerializeField]
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    [SerializeField]
    EventSystem m_EventSystem;

    // Start is called before the first frame update
    void Start()
    {
        isTouching = false;
        Z = transform.position.z;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        checkTouch();
        if (transform.position.z >= maxZ)
        {
            rb.isKinematic = true;
            rb.isKinematic = false;
            transform.position = new Vector3(transform.position.x, transform.position.y, maxZ-0.01f);

        }
        else if (transform.position.z <= minZ)
        {
            rb.isKinematic = true;
            rb.isKinematic = false;
            transform.position = new Vector3(transform.position.x, transform.position.y, minZ+0.01f);
        }
    }
    void checkTouch()
    {
        if (Input.GetMouseButtonDown(0) && !Pointer_Over_UI())
        {
            startTouch = Input.mousePosition;
            moveTouch = Input.mousePosition;
            isTouching = true;
            //Debug.Log("Touch");
        }
        if (Input.GetMouseButton(0) && isTouching)
        {
            moveTouch = Input.mousePosition;
            if (Mathf.Abs(startTouch.y - moveTouch.y) > moveDistanceToScroll)
            {
                MoveCamera();
            }
            startTouch = Input.mousePosition;

        }
        if (Input.GetMouseButtonUp(0))
        {
            startTouch = Vector3.zero;
            moveTouch = Vector3.zero;
            isTouching = false;
        }
    }

    void MoveCamera()
    {
        rb.AddForce(transform.forward * (startTouch.y - moveTouch.y) *2, ForceMode.Acceleration);
    }



    bool Pointer_Over_UI()
    {
        
        
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.layer == 5) 
            {
            //Debug.Log("you clicked on "+ results[i].gameObject.name);
            return true;
            }
        }
        return false;
        
    }
}

