using UnityEngine;
using UnityEngine.UI;
using static BlackBoard;
using static InputManager;

public class DragMinion : DragAndDrop
{
    [SerializeField] GameObject entity;
    [SerializeField] GameObject draggedEntity;
    [SerializeField] GameObject SpawnPoint;
    protected Unit unit;
    protected SpriteRenderer spawnPointSprite;
    protected SpriteRenderer draggedEntitySprite;
    protected SpriteRenderer entitySprite;

    protected override ButtonsState GetDraggedState() => new DragState_Minion(this);
    protected override string ButtonText() => unit.entityName;
    protected override void Drop()
    {
        if (positionValid)
        {
            levelManager.Gold -= unit.goldValue;
            Instantiate(entity, draggedEntity.transform.position - Vector3.up * inputManager.dragHeight, Quaternion.identity);
        }
    }
    protected override void Start()
    {
        unit = entity.GetComponent<Unit>();
        spawnPointSprite = SpawnPoint.GetComponent<SpriteRenderer>();
        draggedEntitySprite = draggedEntity.GetComponentInChildren<SpriteRenderer>();
        entitySprite = entity.GetComponentInChildren<SpriteRenderer>();
        base.Start();
    }

    class ButtonState_Minion : ButtonsState
    {
        protected new DragMinion button => (DragMinion)base.button;
        public ButtonState_Minion(DragAndDrop button) : base(button) { }
        protected override void OnEnable()
        {
            base.OnEnable();
            positionValid = false;
        }
    }
    class DragState_Minion : ButtonState_Minion
    {
        public DragState_Minion(DragAndDrop button) : base(button) { }

        protected override void OnEnable()
        {
            if (button.unit.goldValue > levelManager.Gold)
            {
                Debug.Log("Not enough gold! you only have " + levelManager.Gold + " out of " + button.unit.goldValue);
                inputManager.dragState = null;
                return;
            }
            base.OnEnable();
            button.draggedEntitySprite.sprite = button.entitySprite.sprite;
            button.draggedEntitySprite.color = button.entitySprite.color;
            button.draggedEntity.SetActive(true);
            button.SpawnPoint.SetActive(true);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            button.draggedEntity.SetActive(false);
            button.SpawnPoint.SetActive(false);
            positionValid = false;
        }
        protected override void OnUpdate()
        {
            //Todo: Touch
            //if (Input.touchCount > 0)
            //    pointerPosition = Input.GetTouch(0).position;
            Vector2 pointerPosition = Input.mousePosition;
            button.draggedEntity.transform.position = inputManager.RayToPlanePosition(mainCam.ScreenPointToRay(pointerPosition));
            if (Physics.Raycast(button.draggedEntity.transform.position, Vector3.down, out RaycastHit floorHit, button.draggedEntity.transform.position.y + 0.5f, button.unit.placeableLayers))
            {
                button.SpawnPoint.transform.position = new Vector3(button.draggedEntity.transform.position.x, floorHit.point.y + 0.5f, button.draggedEntity.transform.position.z);
                button.spawnPointSprite.color = Color.green;
                positionValid = true;
            }
            else
            {
                button.SpawnPoint.transform.position = new Vector3(button.draggedEntity.transform.position.x, button.SpawnPoint.transform.position.y, button.draggedEntity.transform.position.z);
                button.spawnPointSprite.color = Color.red;
                positionValid = false;
            }
        }
    }
}