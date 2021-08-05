using CustomAttributes;
using DataSaving;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField]
    private LevelSO _levelSO;
    [SerializeField, FoldoutGroup("Completed color")]
    private ColorBlock _completedColor = ColorBlock.defaultColorBlock;
    [SerializeField]
    private bool _startUnlocked;

    [SerializeField]
    List<LevelButton> AdjecentLevels;

    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnUnlock;

    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnCompleted;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent<ColorBlock> CompletedColor;

    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnLocked;
    LevelsData _levels;
    Level _level;
    GameManager gameManager;
    private void Awake()
    {
        _levels = DataHandler.Load<LevelsData>();
        gameManager = GameManager.instance;

        LoadLevelData();
    }
    private void Start()
    {
        CheckIfLevelWon();
    }
    private void CheckIfLevelWon()
    {
        if (_levelSO == null)
            return;

        LinkedListNode<LevelSO> levelNode = gameManager.levelsWon.Find(_levelSO);
        
        if (levelNode == null)
            return;

        LevelWon();

        levelNode.Value.CompleteAndEarnReward();

        gameManager.levelsWon.Remove(levelNode);
    }

    private void LoadLevelData()
    {
        if (_levelSO == null)
            return;

        var name = _levelSO.SceneName;
        _level = _levels.Levels.Find((x) => name == x.Name);

        if (_level == null)
        {
            _level = new Level(name);
            _level.Name = name;
            _levels.Levels.Add(_level);
        }

        switch (_level.LevelState)
        {
            case LevelState.Unlocked:
                UnlockLevel();
                break;
            case LevelState.Completed:
                LevelWon();
                break;
            case LevelState.Locked:
            default:
                if (_startUnlocked)
                    UnlockLevel();
                break;
        }
    }
    private void LoadLevelToScreen()
    {
        if (_level == null)
            return;

        switch (_level.LevelState)
        {
            case LevelState.Unlocked:
                OnUnlock?.Invoke();
                break;
            case LevelState.Completed:
                OnCompleted?.Invoke();
                CompletedColor?.Invoke(_completedColor);
                break;
            case LevelState.Locked:
                OnLocked?.Invoke();
                break;
        }
    }
    [ContextMenu("Mitz")]
    public void LevelWon()
    {
        if (_level == null)
            return;

        if (_level.LevelState < LevelState.Completed)
            _level.LevelState = LevelState.Completed;

        for (int i = 0; i < AdjecentLevels.Count; i++)
        {
            AdjecentLevels[i].UnlockLevel();
        }

        LoadLevelToScreen();
    }
    public void UnlockLevel()
    {
        if (_level == null)
            return;

        if (_level.LevelState == LevelState.Unlocked)
        {
            LoadLevelToScreen();
            return;
        }

        if (_level.LevelState < LevelState.Unlocked)
            _level.LevelState = LevelState.Unlocked;

        LoadLevelToScreen();
    }
    public void Select() => LevelSelection.SelectedLevel = _levelSO;
}
public enum LevelState { Locked, Unlocked, Completed }
[Serializable]
public class Level : DirtyData
{
    [SerializeField]
    private string _name;
    public string Name { get => _name; set => Setter(ref _name, value); }

    [SerializeField]
    private LevelState _levelState = LevelState.Locked;

    public Level(string name, LevelState levelState = LevelState.Locked)
    {
        Name = name;
        LevelState = levelState;
    }

    public LevelState LevelState { get => _levelState; set => Setter(ref _levelState, value); }

}