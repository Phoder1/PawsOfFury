using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum DragState { None, Pressed, Dragged }
public class UnitButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IScrollHandler
{
    [SerializeField]
    private Image _raycastTarget;
    private Canvas _canvas;
    private LayoutGroup _layoutGroup;
    private ScrollRect _scrollRect;
    private EventTrigger _contentEventTrigger;
    private Image _contentBackgroundImage;


    private Transform _parent;
    private int _siblingIndex;
    private int _pointerID;
    private DragState _state = DragState.None;
    public DragState State
    {
        get => _state;
        set
        {
            if (value == _state)
                return;

            OnDisable();

            _state = value;

            OnEnable();

            void OnDisable()
            {
                switch (_state)
                {
                    case DragState.None:
                        break;
                    case DragState.Pressed:
                        _raycastTarget.raycastTarget = true;
                        break;
                    case DragState.Dragged:
                        transform.SetParent(_parent);
                        transform.SetSiblingIndex(_siblingIndex);
                        //_layoutGroup.enabled = true;
                        break;
                }
            }

            void OnEnable()
            {
                switch (_state)
                {
                    case DragState.None:
                        break;
                    case DragState.Pressed:
                        _raycastTarget.raycastTarget = false;
                        break;
                    case DragState.Dragged:
                        //_layoutGroup.enabled = false;
                        transform.SetParent(_canvas.transform, true);
                        transform.SetAsLastSibling();
                        break;
                }
            }
        }
    }
    #region Unity callbacks
    void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _scrollRect = GetComponentInParent<ScrollRect>();
        //_contentEventTrigger = transform.parent.parent.GetComponentInChildren<EventTrigger>();
        _layoutGroup = GetComponentInParent<LayoutGroup>();
        //_contentEventTrigger.triggers.Find((x) => x.eventID == EventTriggerType.PointerExit)?.callback.AddListener(OnContentPointerExit);
        _contentBackgroundImage = transform.parent.parent.GetComponent<Image>();
    }
    void Start()
    {
        _parent = transform.parent;
        _siblingIndex = transform.GetSiblingIndex();
    }
    void Update()
    {

    }
    #endregion
    #region Pointer callbacks
    public void OnPointerDown(PointerEventData eventData)
    {
        State = DragState.Pressed;
        Debug.Log("pressed");
    }
    private void OnContentPointerExit(BaseEventData baseEventData)
    {
        Debug.Log("Exit");

        if (State != DragState.Pressed)
            return;

        var eventData = (PointerEventData)baseEventData;
        var pointerId = eventData.pointerId;

        if (pointerId == -1 || pointerId >= 0)
        {
            State = DragState.Dragged;
            eventData.pointerDrag = gameObject;
            eventData.dragging = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        State = DragState.None;
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (State == DragState.Pressed)
        {
            _scrollRect.OnDrag(eventData);
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            if (!raycastResults.Exists((x) => x.gameObject == _contentBackgroundImage.gameObject))
            {
                var rectTransform = _contentBackgroundImage.rectTransform;
                var min = rectTransform.rect.min + (Vector2)rectTransform.position;
                var max = rectTransform.rect.max + (Vector2)rectTransform.position;
                if (eventData.position.x < max.x && eventData.position.x > min.x
                    && eventData.position.y > max.y)
                {
                    State = DragState.Dragged;
                    _pointerID = eventData.pointerId;
                }
            }
        }
        if (State == DragState.Dragged)
        {
            Vector3 pos = transform.position;

            if (_pointerID == -1)
                pos = Input.mousePosition;

            if (_pointerID >= 0)
                pos = Input.GetTouch(_pointerID).position;

            transform.position = pos;
        }
        else
            _scrollRect.OnDrag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (State != DragState.Dragged)
            _scrollRect.OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (State != DragState.Dragged)
            _scrollRect.OnEndDrag(eventData);
    }

    public void OnScroll(PointerEventData data)
    {
        if (State != DragState.Dragged)
            _scrollRect.OnScroll(data);
    }
    #endregion
}
