using CustomAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static IngameBlackBoard;
using static InputManager;

public abstract class DragAndDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] TextMeshProUGUI goldText;
    [LocalComponent(true)] [SerializeField] protected Button uiButton;
    [SerializeField] protected Image image;


    protected static Camera mainCam;
    protected abstract ButtonsState GetDraggedState();
    protected abstract void Drop();
    protected abstract int goldValue();
    protected abstract Sprite Sprite();
    protected virtual Color SpriteColor() => Color.white;
    protected static bool positionValid;
    void GoldAmountChanged(float goldAmount) => uiButton.interactable = CheckSpawnValid(goldAmount);
    protected bool CheckSpawnValid(float goldAmount) => goldValue() <= goldAmount;
    protected virtual void Start()
    {
        mainCam = Camera.main;
        goldText.text = goldValue().ToString();
        image.sprite = Sprite();
        image.color = SpriteColor();
        levelManager.GoldAmountChanged += GoldAmountChanged;
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
