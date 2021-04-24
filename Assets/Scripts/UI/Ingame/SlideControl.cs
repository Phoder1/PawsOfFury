using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CustomAttributes;

public class SlideControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [LocalComponent] [SerializeField] Canvas canvas;
    bool dragged;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerPressRaycast.gameObject == gameObject)
            dragged = true;
    }

    public void OnPointerUp(PointerEventData eventData) => StopDrag();
    void StopDrag()
    {
        if (dragged == false)
            return;
        dragged = false;
    }

    private void Update()
    {
        if (dragged)
        {
            if (Input.GetMouseButton(0))
                transform.position = Input.mousePosition;
            else if (Input.touchCount != 0)
                transform.position = Input.GetTouch(0).position;
            else
                StopDrag();
        }
    }
    void UpdatePointerPosition(Vector3 pointerPosition)
    {
        transform.position = pointerPosition;
    }
}