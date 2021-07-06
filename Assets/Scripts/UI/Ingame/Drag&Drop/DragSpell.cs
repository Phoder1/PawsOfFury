using UnityEngine;
using static IngameBlackBoard;
using static InputManager;

public class DragSpell : DragAndDrop
{
    [SerializeField] GameObject spellPrefab;
    [SerializeField] GameObject shadow;
    [SerializeField] LayerMask pathLayer;
    CastableSpell spell;

    protected ShadowController shadowController;
    protected override int GoldValue() => spell.goldValue;
    protected override ButtonsState GetDraggedState() => new DragState_Spell(this);
    protected override Sprite Sprite() => spell.buttonSprite;
    protected override void Drop()
    {
        GameObject spawnedSpell = Instantiate(spellPrefab, shadow.transform.position, Quaternion.identity);
        CastableSpell spellScript = spawnedSpell.GetComponent<CastableSpell>();
        spellScript.Init();
    }


    protected override void Start()
    {
        spell = spellPrefab.GetComponent<CastableSpell>();
        shadowController = shadow.GetComponent<ShadowController>();
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
            button.shadow.SetActive(true);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            button.shadow.SetActive(false);
            positionValid = false;
        }
        protected override void OnUpdate()
        {
            Ray mouseRay = MainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit floorHit, MainCam.farClipPlane, button.pathLayer))
            {
                button.shadow.transform.position = floorHit.point + Vector3.up * 0.4f;
                button.shadowController.Color = Color.green;
                positionValid = true;
            }
            else
            {
                button.shadow.transform.position = inputManager.RayToPlanePosition(mouseRay, button.shadow.transform.position.y);
                button.shadowController.Color = Color.red;
                positionValid = false;
            }
        }
    }
}
