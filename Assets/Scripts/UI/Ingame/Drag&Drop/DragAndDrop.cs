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
    [LocalComponent(getComponentFromChildrens: true)] [SerializeField] protected Button uiButton;
    [SerializeField] protected Image image;
    protected static Camera MainCam => GameManager.MainCam;
    protected abstract ButtonsState GetDraggedState();
    protected abstract void Drop();
    protected abstract int GoldValue();
    protected abstract Sprite Sprite();
    protected virtual Color SpriteColor() => Color.white;
    protected static bool positionValid;


    void UpdateGoldAmount(float goldAmount) => uiButton.interactable = CheckSpawnValid(goldAmount);
    protected bool CheckSpawnValid(float goldAmount) => GoldValue() <= goldAmount;
    protected virtual void Start()
    {
        if (goldText != null)
            goldText.text = GoldValue().ToString();
        if (image != null)
        {
            image.sprite = Sprite();
            image.color = SpriteColor();
        }
        levelManager.GoldAmountChanged += UpdateGoldAmount;
        UpdateGoldAmount(levelManager.Gold);
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
