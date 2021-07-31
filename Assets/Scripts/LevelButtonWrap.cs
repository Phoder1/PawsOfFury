using CustomAttributes;
using DataSaving;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LevelButton), typeof(Button))]
public class LevelButtonWrap : MonoBehaviour
{
    [SerializeField, LocalComponent]
    private LevelButton _levelButton;
    [SerializeField, LocalComponent]
    private Button button;

    [SerializeField]
    private Color ColorForButton;
    [SerializeField]
    private bool _startUnlocked;

    [SerializeField]
    List<LevelButtonWrap> AdjecentLevels;

    LevelsData _levels;
    Level _level;
    GameManager gameManager;
    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
        _levels = DataHandler.Load<LevelsData>();
        gameManager = GameManager.instance;
        CheckIfLevelWon();

        LoadLevelData();

    }
    private void CheckIfLevelWon()
    {
        if (_levelButton == null || _levelButton.Level == null)
            return;

        LinkedListNode<LevelSO> levelNode = gameManager.levelsWon.Find(_levelButton.Level);
        
        if (levelNode == null)
            return;

        LevelWon();

        levelNode.Value.CompleteAndEarnReward();

        gameManager.levelsWon.Remove(levelNode);
    }

    private void LoadLevelData()
    {
        if (_levelButton == null || _levelButton.Level == null)
            return;

        var name = _levelButton.Level.SceneName;
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
                button.interactable = true;
                break;
            case LevelState.Completed:
                ColorBlock tempBlock = button.colors;
                tempBlock.normalColor = ColorForButton;
                button.colors = tempBlock;
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