using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

public class WindowSwipe : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField, ShowIf("_enableHorizontalSwiping")]
    private UiWindow _uiWndow_left;
    [SerializeField, ShowIf("_enableHorizontalSwiping")]
    private UiWindow _uiWndow_right;

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
