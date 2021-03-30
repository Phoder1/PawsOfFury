using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using static BlackBoard;
using static InputManager;

public abstract class DragAndDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] TextMeshProUGUI text;


    protected static Camera mainCam;
    protected abstract ButtonsState GetDraggedState();
    protected abstract void Drop();
    protected abstract string ButtonText();
    protected static bool positionValid;
    protected virtual void Start()
    {
        mainCam = Camera.main;
        text.text = ButtonText();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        positionValid = false;
        if (inputManager.PressState == PressState.DefaultState)
        {
            inputManager.PressState = PressState.Pressed;
            inputManager.dragState = GetDraggedState();
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (inputManager.PressState == PressState.Pressed)
        {
            inputManager.PressState = PressState.Dragging;
            inputManager.StartDragging();
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (inputManager.PressState == PressState.Dragging)
            Drop();
        inputManager.PressState = PressState.DefaultState;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        positionValid = false;
        if (inputManager.PressState == PressState.Dragging)
        {
            inputManager.PressState = PressState.Pressed;
        }
    }
    protected TileBase GetTileAtPosition(Vector3 worldPosition)
    {
        worldPosition.y = 0;
        return levelManager.tilemap.GetTile(levelManager.tilemap.WorldToCell(worldPosition));
    }
}
