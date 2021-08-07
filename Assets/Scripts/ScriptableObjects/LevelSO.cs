using DataSaving;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = Database.SODataFol + "Level")]
[InlineEditor]
public class LevelSO : ScriptableObject
{
    [SerializeField, ValueDropdown("Scenes"), InspectorName("Scene")]
    private string _sceneName;
    [SerializeField]
    private string _levelName = string.Empty;
    [SerializeField]
    private bool _replayable = true;
    [BoxGroup("Rewards")]
    [SerializeField, Range(0, 1)]
    private float _chanceToGetACard;
    [BoxGroup("Rewards")]
    [SerializeField]
    private UnitSO _overrideReward;
    [BoxGroup("Rewards")]
    [SerializeField]
    private int _crystalsReward = 0;

    [NonSerialized]
    private LevelsData _levelsData = null;

    private LevelsData LevelsData => DataHandler.Getter(ref _levelsData);
    [NonSerialized]
    private PlayerCurrency _playerCurrency = null;
    private PlayerCurrency PlayerCurrency => DataHandler.Getter(ref _playerCurrency);

    [NonSerialized]
    private InventoryData _inventory;
    public InventoryData Inventory => DataHandler.Getter(ref _inventory);
    public string SceneName => _sceneName;
    public string LevelName => _levelName == string.Empty ? StringFormating.SplitCamelCase(name) : _levelName;
    public float ChanceToGetACard => _chanceToGetACard;
    public int CrystalsReward => _crystalsReward;
    public Level GetLevel() => LevelsData.GetLevel(SceneName);
    public bool Replayable => _replayable;
    public bool IsCompleted
    {
        get
        {
            var level = GetLevel();

            if (level == null || level.LevelState != LevelState.Completed)
                return false;

            return true;

        }
    }
    public (int crystals, UnitSO unit) CompleteAndEarnReward()
    {
        CompletedLevel();
        return EarnReward();
    }
    public void CompletedLevel()
    {
        var level = GetLevel();

        if (level == null)
        {
            LevelsData.Levels.Add(new Level(SceneName, LevelState.Completed));
        }
        else if (level.LevelState != LevelState.Completed)
        {
            level.LevelState = LevelState.Completed;
        }
    }
    private (int crystals, UnitSO unit) EarnReward()
    {
        PlayerCurrency.Crystals += CrystalsReward;

        UnitSO rewardUnit = _overrideReward;

        if (rewardUnit == null && ChanceToGetACard > 0 && UnityEngine.Random.value <= ChanceToGetACard)
            rewardUnit = UnitRandomizer.GetRandomUnit(1);

        if (rewardUnit != null)
            Inventory.AddUnits(rewardUnit, 1);

        return (CrystalsReward, rewardUnit);
    }
#if UNITY_EDITOR
    private string[] Scenes
    {
        get
        {
            var scenes = UnityEditor.EditorBuildSettings.scenes;
            string[] sceneNames = new string[scenes.Length];

            for (int i = 0; i < scenes.Length; i++)
                sceneNames[i] = GetFileName(scenes[i].path);

            return sceneNames;
        }
    }


    private string GetFileName(string path)
    {
        var splitPath = path.Split('/');
        splitPath = splitPath[splitPath.Length - 1].Split('.');
        return splitPath[splitPath.Length - 2];
    }
#endif
}
