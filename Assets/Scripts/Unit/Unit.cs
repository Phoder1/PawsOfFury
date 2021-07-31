using Assets.Stats;
using CustomAttributes;
using UnityEngine;
using static IngameBlackBoard;

[RequireComponent(typeof(NavScript))]
public class Unit : Entity
{
    [SerializeField]
    protected LayerMask unlockablePathLayer;

    [LocalComponent(getComponentFromChildrens: true)] public SpriteRenderer buttonSpriteRenderer;
    public LayerMask placeableLayers;
    protected override EntityState DefaultState() => new WalkState(this);
    protected virtual EntityState AttackingState => new AttackState(this);
    protected NavScript navScript;


    protected override void Start()
    {
        navScript = GetComponent<NavScript>();
        base.Start();
        navScript.Speed = stats.GetStatValue(StatType.WalkSpeed);
    }

    protected override void OnTargetLoss() => stateMachine.State = new WalkState(this);
    protected override void DetectedOutOfRange(Entity detectedEntity)
    {
        navScript.StartMove(detectedEntity.transform.position);
    }

    public override void DetectedInRange(EntityHit detectedEntity) => navScript.StopMove();

    class UnitState : EntityState
    {
        public UnitState(Unit unit) : base(unit) { }
        protected Unit Unit => (Unit)entity;
    }
    class WalkState : UnitState
    {
        public WalkState(Unit unit) : base(unit) { }
        protected override void OnEnable()
        {
            if (levelManager.LevelEndPos != null)
                Unit.navScript.StartMove(levelManager.LevelEndPos);
        }
        protected override void OnUpdate()
        {
            detectedEntity = Targets.GetEntityHit(entity, entity.projectile.detection);
            if (detectedEntity != null && detectedEntity.entity != null)
                Unit.stateMachine.State = Unit.AttackingState;

            if (Physics.Raycast(Unit.transform.position, Vector3.down, out RaycastHit floorHit, 0.5f, Unit.unlockablePathLayer))
            {
                var unlockable = floorHit.collider.GetComponent<UnlockPlaceablePath>();

                if (unlockable != null)
                    unlockable.Unlock();
            }
        }

        protected override void OnDisable() => Unit.navScript.StopMove();
    }
}

