using TMPro;
using UnityEngine;

public class DragMinion : DragAndDrop
{
    [SerializeField] GameObject minion;
    [SerializeField] GameObject draggedObject;
    [SerializeField] GameObject SpawnPoint;
    [SerializeField] LayerMask pathLayer;
    protected SpriteRenderer spawnPointSprite;
    protected override ButtonState GetDefaultState() => null;
    protected override ButtonState GetDraggedState() => new DragState_Minion(this);
    protected override ButtonState GetPressedState() => null;
    protected override string ButtonText() => minion.GetComponent<Unit>().entityName;
    protected override void Drop() => Instantiate(minion, draggedObject.transform.position - Vector3.up * InputManager._instance.dragHeight, Quaternion.identity);
    protected override void Start()
    {
        spawnPointSprite = SpawnPoint.GetComponent<SpriteRenderer>();
        base.Start();
    }

    class ButtonState_Minion : ButtonState
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
            base.OnEnable();
            button.draggedObject.SetActive(true);
            button.SpawnPoint.SetActive(true);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            button.draggedObject.SetActive(false);
            button.SpawnPoint.SetActive(false);
            positionValid = false;
        }
        protected override void OnUpdate()
        {
            button.draggedObject.transform.position = inputManager.RayToPlanePosition(mainCam.ScreenPointToRay(Input.mousePosition));
            if (Physics.Raycast(button.draggedObject.transform.position, Vector3.down, out RaycastHit floorHit, button.draggedObject.transform.position.y + 0.5f, button.pathLayer))
            {
                button.SpawnPoint.transform.position = new Vector3(button.draggedObject.transform.position.x, floorHit.point.y + 0.5f, button.draggedObject.transform.position.z);
                button.spawnPointSprite.color = Color.green;
                positionValid = true;
            }
            else
            {
                button.SpawnPoint.transform.position = new Vector3(button.draggedObject.transform.position.x, button.SpawnPoint.transform.position.y, button.draggedObject.transform.position.z);
                button.spawnPointSprite.color = Color.red;
                positionValid = false;
            }
        }
    }
}