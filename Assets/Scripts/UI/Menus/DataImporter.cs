using Refrences;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

public enum DataTypes { Any, String, Int, Float, Bool, ObjectRefrence, UnitSO, }
[Flags] public enum TriggerCallbacks { None = 0, Manual = 1, Awake = 2, Start = 4, OnEnable = 8, OnDisable = 16 }
public class DataImporter : MonoBehaviour
{
    [SerializeField, EnumToggleButtons, HideLabel]
    private TriggerCallbacks ControlCallbacks = TriggerCallbacks.Manual;

    [SerializeField, Tooltip("The default data to use, also support refrences.")]
    private UnityEngine.Object _defaultData;
    public UnityEngine.Object DefaultData
    {
        get => _defaultData;
        set
        {
            if (_defaultData == value)
                return;

            if (_defaultData is ObjectRefrence _originalRef && _originalRef != null)
                _originalRef.OnValueChanged -= ApplyImport;

            _defaultData = value;

            if (_autoImport && _defaultData is ObjectRefrence _newRef && _newRef != null)
                _newRef.OnValueChanged += ApplyImport;
        }
    }

    [SerializeField, ShowIf("@DefaultData is ObjectRefrence"), Tooltip("Auto imports data on value changes")]
    private bool _autoImport;

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
    [ShowIf("@_dataType == DataTypes.ObjectRefrence")]
    public UnityEvent<ObjectRefrence> ObjectRefrenceExport;
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
    private void Start()
    {
        TryDefaultImport(TriggerCallbacks.Start);

        if (_autoImport && _defaultData is ObjectRefrence _ref && _ref != null)
            _ref.OnValueChanged += ApplyImport;

    }
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
            ApplyImport(DefaultData);
    }
    private bool TryDefaultImport(TriggerCallbacks trigger)
    {
        if (ControlCallbacks.HasFlag(trigger))
        {
            ApplyImport(DefaultData);
            return true;
        }
        return false;
    }
    private void ApplyImport(object data)
    {
        if (data is Refrences.ObjectRefrence _ref)
        {
            ObjectRefrenceExport?.Invoke(_ref);
            data = _ref.Value;
        }
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
