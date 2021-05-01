using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

public enum DataTypes { Any, String, Int, Float, Bool, UnitSO, }
[Flags] public enum TriggerCallbacks { None = 0, Manual = 1, Awake = 2, Start = 4, OnEnable = 8, OnDisable = 16 }
public class DataImporter : MonoBehaviour
{
    [SerializeField, EnumToggleButtons, HideLabel]
    private TriggerCallbacks ControlCallbacks = TriggerCallbacks.Manual;
    [SerializeField]
    private UnityEngine.Object _defaultData = default;
    [SerializeField, EnumPaging]
    private DataTypes _dataType;
    #region SimpleTypes
    [ShowIf("@_dataType == DataTypes.Any")]
    public UnityEvent<object> DefaultPassthrough;
    [ShowIf("@_dataType == DataTypes.String")]
    public UnityEvent<string> StringExport;
    [ShowIf("@_dataType == DataTypes.Int")]
    public UnityEvent<int> IntExport;
    [ShowIf("@_dataType == DataTypes.Float")]
    public UnityEvent<float> FloatExport;
    [ShowIf("@_dataType == DataTypes.Bool")]
    public UnityEvent<bool> BoolExport;
    #endregion
    #region UnitSO
    [FoldoutGroup("Unit Export events", VisibleIf = "@_dataType == DataTypes.UnitSO"),]
    public UnityEvent<UnitSO> UnitSoPassthrough;
    [FoldoutGroup("Unit Export events")]
    public UnityEvent<int> IdExport;
    [FoldoutGroup("Unit Export events")]
    public UnityEvent<int> TierExport;
    [FoldoutGroup("Unit Export events")]
    public UnityEvent<string> DescriptionExport;
    [FoldoutGroup("Unit Export events")]
    public UnityEvent<int> CountExport;
    [FoldoutGroup("Unit Export events")]
    public UnityEvent<Sprite> UnitSpriteExport;
    [FoldoutGroup("Unit Export events")]
    public UnityEvent<Sprite> TierCrystalExport;
    [FoldoutGroup("Unit Export events")]
    public UnityEvent<bool> OwnedExport;
    #endregion
    private void Awake() => TryDefaultImport(TriggerCallbacks.Awake);
    private void Start() => TryDefaultImport(TriggerCallbacks.Start);
    private void OnEnable() => TryDefaultImport(TriggerCallbacks.OnEnable);
    private void OnDisable() => TryDefaultImport(TriggerCallbacks.OnDisable);
    public void Import(object data)
    {
        if (ControlCallbacks.HasFlag(TriggerCallbacks.Manual))
            ApplyImport(data);
    }
    public void ImportDefault()
    {
        if (ControlCallbacks.HasFlag(TriggerCallbacks.Manual))
            ApplyImport(_defaultData);
    }
    private bool TryDefaultImport(TriggerCallbacks trigger)
    {
        if (ControlCallbacks.HasFlag(trigger))
        {
            ApplyImport(_defaultData);
            return true;
        }
        return false;
    }
    private void ApplyImport(object data)
    {
        DefaultPassthrough?.Invoke(data);
        if (data != null)
            switch (data)
            {
                case string _string:
                    StringExport?.Invoke(_string);
                    break;
                case int _int:
                    IntExport?.Invoke(_int);
                    break;
                case float _float:
                    FloatExport?.Invoke(_float);
                    break;
                case bool _bool:
                    BoolExport?.Invoke(_bool);
                    break;
                case UnitSO unitData:
                    DescriptionExport?.Invoke(unitData.Description);
                    IdExport?.Invoke(unitData.ID);
                    TierExport?.Invoke(unitData.Tier);
                    CountExport?.Invoke(unitData.Count);
                    UnitSpriteExport?.Invoke(unitData.UiSprite);
                    TierCrystalExport?.Invoke(unitData.TierCrystal);
                    OwnedExport?.Invoke(unitData.Owned);
                    break;
            }
    }
    [Serializable]
    public class IntDataExportEvent : UnityEvent<int> { }
    [Serializable]
    public class ClipDataExportEvent : UnityEvent<AnimationClip> { }
    [Serializable]
    public class StringDataExportEvent : UnityEvent<string> { }
}
