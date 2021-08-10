using DataSaving;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum DragState { None, Pressed, Dragged }
public class MinionBookUnitButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IScrollHandler
{
    [SerializeField]
    private Image _raycastTarget;
    [SerializeField, Tooltip("In screen heights per second")]
    private float _maxSpeed = 3f;
    [SerializeField, Tooltip("In cards ratio")]
    private float _minSnapDistance = 1f;
    [SerializeField]
    private bool _disableScrollRectOnDrag = false;
    [SerializeField, Tooltip("The max distance (in card heights) before it counts as drag")]
    private float _maxPressDistance = 0.3f;

    [SerializeField]
    private UnityEvent OnPress;
    [SerializeField]
    private UnityEvent OnSelectUE;
    [SerializeField]
    private UnityEvent<UnitInformation> OnValueChanged;

    [Space]
    [SerializeField]
    private TagFilter _teamTag;
    private UnitInformation _unit;
    public UnitInformation Unit { get => _unit; set => _unit = value; }
    private Canvas _canvas;
    private LayoutGroup _layoutGroup;
    private ScrollRect _scrollRect;
    private EventTrigger _contentEventTrigger;
    private Image _contentBackgroundImage;
    private RectTransform _rectTransform;

    private Transform _parent;
    private int _siblingIndex;
    private int _pointerID;
    private DragState _state = DragState.None;
    private Vector3 _dragLastPosition;
    private float _cardHeight;
    private float _dragDistance;
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
                        if (value != DragState.Dragged && _dragDistance <= _maxPressDistance * _cardHeight)
                        {
                            OnPress?.Invoke();
                            Select();
                        }
                        break;
                    case DragState.Dragged:
                        transform.SetParent(_parent);
                        transform.SetSiblingIndex(_siblingIndex);
                        if (_disableScrollRectOnDrag)
                            _layoutGroup.enabled = true;
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
                        _dragLastPosition = transform.position;
                        break;
                    case DragState.Dragged:
                        if (_disableScrollRectOnDrag)
                            _layoutGroup.enabled = false;
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
        _rectTransform = GetComponent<RectTransform>();
        _cardHeight = _rectTransform.rect.height;

        DataHandler.Load<InventoryData>().OnValueChange += () => OnValueChanged?.Invoke(Unit);
    }
    void Start()
    {
        _parent = transform.parent;
        _siblingIndex = transform.GetSiblingIndex();
    }
    void Update()
    {
        if (State == DragState.Dragged)
        {
            Vector3 targetPos = transform.position;

            if (_pointerID == -1)
                targetPos = Input.mousePosition;

            if (_pointerID >= 0)
                targetPos = Input.GetTouch(_pointerID).position;

            float maxSpeed = _maxSpeed * Screen.height;
            Vector3 movement = Vector3.ClampMagnitude(targetPos - transform.position, maxSpeed * Time.deltaTime);
            transform.position += movement;
        }
    }
    #endregion
    #region Pointer callbacks
    public void OnPointerDown(PointerEventData eventData)
    {
        State = DragState.Pressed;
    }
    private void OnContentPointerExit(BaseEventData baseEventData)
    {
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
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        var teamUnit = raycastResults.Find((x) => _teamTag.Contains(x.gameObject.tag)).gameObject;

        UiUnitController ctrl;
        if (teamUnit != null && (ctrl = teamUnit.GetComponentInParent<UiUnitController>()) != null)
            ctrl.UnitInfo = _unit;

        State = DragState.None;
        _dragDistance = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _dragDistance += (transform.position - _dragLastPosition).magnitude;

        if (State == DragState.Pressed)
        {
            if (Input.mousePosition.y > transform.position.y + _rectTransform.rect.height * _minSnapDistance)
            {
                State = DragState.Dragged;
                _pointerID = eventData.pointerId;
            }
            else
            {
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
        }
        if (State != DragState.Dragged)
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
    public void Select()
    {
        UnitSelection.LastSelectedUnit = Unit;
        OnSelectUE?.Invoke();
    }
    public void Deselect()
    {
        if (UnitSelection.LastSelectedUnit == Unit)
            UnitSelection.LastSelectedUnit = null;
    }
    #endregion
}
