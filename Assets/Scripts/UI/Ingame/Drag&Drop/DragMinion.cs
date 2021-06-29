using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using static IngameBlackBoard;
using static InputManager;
using CustomAttributes;

public class DragMinion : DragAndDrop
{
    [SerializeField] GameObject draggedEntity;
    [Tooltip("shadow")]
    [SerializeField] GameObject shadow;
    [Suffix("Uu/s", toolTip: "Unity units per second")]
    [SerializeField] float dropSpeed;

    private GameObject unitPrefab;
    protected Unit unit;
    protected ShadowController shadowController;
    protected SpriteRenderer draggedEntitySprite;
    protected SpriteRenderer entitySprite;
    private UnitInformation unitInformation;

    public UnitInformation UnitInformation { get => unitInformation; set => unitInformation = value; }

    protected override ButtonsState GetDraggedState() => new DragState_Minion(this);
    protected override int goldValue() => unit.goldValue;
    protected override Sprite Sprite() => unit.buttonSpriteRenderer.sprite;
    protected override Color SpriteColor() => unit.buttonSpriteRenderer.color;
    protected override void Drop()
    {
        if (positionValid)
        {
            levelManager.Gold -= unit.goldValue;
            Vector3 spawnPoint = draggedEntity.transform.position;
            GameObject spawnedUnit = Instantiate(unitPrefab, spawnPoint, Quaternion.identity);
            Vector3 targetPosition = shadow.transform.position + new Vector3(0, 0.5f, -0.5f);
            spawnedUnit.transform.DOMove(targetPosition, Vector3.Distance(spawnedUnit.transform.position, targetPosition) / dropSpeed).OnComplete(() => { InitUnit(spawnedUnit); });

        }

        static void InitUnit(GameObject gameObject) => gameObject.GetComponent<Unit>().Init();
    }
    protected override void Start()
    {
        unitPrefab = UnitInformation.unitSO.GetPrefab(unitInformation.slotData?.Level ?? 1);
        unit = unitPrefab.GetComponent<Unit>();
        shadowController = shadow.GetComponent<ShadowController>();
        draggedEntitySprite = draggedEntity.GetComponentInChildren<SpriteRenderer>();
        entitySprite = unitPrefab.GetComponentInChildren<SpriteRenderer>();
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
            button.shadow.SetActive(true);
            
            button.uiButton.Select();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            button.draggedEntity.SetActive(false);
            button.shadow.SetActive(false);
            positionValid = false;
            EventSystem.current.SetSelectedGameObject(null);
        }
        protected override void OnUpdate()
        {
            //Todo: Touch
            //if (Input.touchCount > 0)
            //    pointerPosition = Input.GetTouch(0).position;
            Vector2 pointerPosition = new Vector2(Input.mousePosition.x, (Input.mousePosition.y + 200f));
            button.draggedEntity.transform.position = inputManager.RayToPlanePosition(MainCam.ScreenPointToRay(pointerPosition));
            if (Physics.Raycast(button.draggedEntity.transform.position, Vector3.down, out RaycastHit floorHit, button.draggedEntity.transform.position.y + 0.5f, button.unit.placeableLayers))
            {
                button.shadow.transform.position = new Vector3(button.draggedEntity.transform.position.x, floorHit.point.y + 0.1f, button.draggedEntity.transform.position.z);
                button.shadowController.Color = Color.green;
                positionValid = true;
            }
            else
            {
                button.shadow.transform.position = new Vector3(button.draggedEntity.transform.position.x, button.shadow.transform.position.y, button.draggedEntity.transform.position.z);
                button.shadowController.Color = Color.red;
                positionValid = false;
            }
        }
    }
}