using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

public class WindowSwipe : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    private bool _enableHorizontalSwiping = true;
    [SerializeField, ShowIf("_enableHorizontalSwiping")]
    private UiWindow _uiWndow_left;
    [SerializeField, ShowIf("_enableHorizontalSwiping")]
    private UiWindow _uiWndow_right;

    [SerializeField]
    private bool _enableVerticalSwiping = true;
    [SerializeField, ShowIf("_enableVerticalSwiping")]
    private UiWindow _uiWndow_up;
    [SerializeField, ShowIf("_enableVerticalSwiping")]
    private UiWindow _uiWndow_down;

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
