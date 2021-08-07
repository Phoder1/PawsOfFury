using DataSaving;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class IncubatorHolder : MonoBehaviour
{
    [SerializeField]
    private MergeHolder _mergeHolder;

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private string _triggerName;

    [SerializeField]
    private float _animationDuration;

    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnRewardReady;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<UnitSO> OnRewardHatching;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnReset;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<Color> CrystalColor;


    private bool _hasReward = false;
    private int _tier = 0;
    private InventoryData _inventory;
    public InventoryData Inventory => DataHandler.Getter(ref _inventory);

    private void OnEnable()
    {
        if (_hasReward && _tier != 0)
        {
            OnRewardReady?.Invoke();
            InvokeCrystalColor();
        }
        else
            ResetIncubator();
    }
    private void OnDestroy()
    {
        if(_hasReward && _tier != 0)
            GenerateReward();
    }
    public void AddReward(UnitInformation unit) => AddReward(unit.unitSO.Tier);
    public void AddReward(UnitSO unit) => AddReward(unit.Tier);
    public void AddReward(int tier)
    {
        _hasReward = true;
        OnRewardReady?.Invoke();

        _tier = tier;
        InvokeCrystalColor();
    }

    private void InvokeCrystalColor() => CrystalColor?.Invoke(UnitSO.GetTierAsset(_tier).BorderColor);

    public void TryHatching()
    {
        if (!_hasReward || _tier == 0)
            return;

        if (_animator != null && !string.IsNullOrWhiteSpace(_triggerName))
            _animator.SetTrigger(_triggerName);

        UnitSO unitReward = GenerateReward();

        OnRewardHatching?.Invoke(unitReward);
        Invoke(nameof(RewardFinishedHatching), _animationDuration);
    }

    private UnitSO GenerateReward()
    {
        var unitReward = UnitRandomizer.GetRandomUnit(_tier);
        Inventory.AddUnits(unitReward, 1);
        return unitReward;
    }

    private void RewardFinishedHatching()
    {
        ResetIncubator();

        if (_mergeHolder != null)
            _mergeHolder.FinishedMerging();
        else
            throw new System.Exception("No merger refrence!");
    }

    private void ResetIncubator()
    {
        _hasReward = false;
        _tier = 0;

        OnReset?.Invoke();
    }
}
