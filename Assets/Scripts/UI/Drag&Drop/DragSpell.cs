using UnityEngine;
using static BlackBoard;
using static InputManager;

public class DragSpell : DragAndDrop
{
    [SerializeField] GameObject spell;
    [SerializeField] GameObject SpawnPoint;
    [SerializeField] LayerMask pathLayer;
    protected SpriteRenderer spawnPointSprite;
    protected override string ButtonText() => spell.GetComponent<CastableSpell>().spellName;
    protected override ButtonsState GetDraggedState() => new DragState_Spell(this);
    protected override void Drop()
    {
        GameObject spellObj = Instantiate(spell, SpawnPoint.transform.position, Quaternion.identity);
        CastableSpell spellScript = spellObj.GetComponent<CastableSpell>();
        spellScript.Init();
    }


    protected override void Start()
    {
        spawnPointSprite = SpawnPoint.GetComponent<SpriteRenderer>();
        base.Start();
    }
    class ButtonState_Spell : ButtonsState
    {
        protected new DragSpell button => (DragSpell)base.button;
        public ButtonState_Spell(DragAndDrop button) : base(button) { }
        protected override void OnEnable()
        {
            base.OnEnable();
            positionValid = false;
        }
    }
    class DragState_Spell : ButtonState_Spell
    {
        public DragState_Spell(DragAndDrop button) : base(button) { }
        protected override void OnEnable()
        {
            base.OnEnable();
            button.SpawnPoint.SetActive(true);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            button.SpawnPoint.SetActive(false);
            positionValid = false;
        }
        protected override void OnUpdate()
        {
            Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit floorHit, mainCam.farClipPlane, button.pathLayer))
            {
                button.SpawnPoint.transform.position = floorHit.point + Vector3.up * 0.4f;
                button.spawnPointSprite.color = Color.green;
                positionValid = true;
            }
            else
            {
                button.SpawnPoint.transform.position = inputManager.RayToPlanePosition(mouseRay, button.SpawnPoint.transform.position.y);
                button.spawnPointSprite.color = Color.red;
                positionValid = false;
            }
        }
    }
}
