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
    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
        _levels = DataHandler.Load<LevelsData>();
        LoadLevelData();
        LoadLevelToScreen();
    }
    private void LoadLevelData()
    {
        if (_levelButton == null || _levelButton.Level == null)
            return;

        var name = _levelButton.Level.SceneName;
        _level = _levels.Levels.Find((x) => name == x.Name);

        if (_level == null)
        {
            _level = new Level();
            _level.Name = name;
            _levels.Levels.Add(_level);
        }
    }
    private void LoadLevelToScreen()
    {
        if (_level == null)
            return;

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
    [ContextMenu("Mitz")]
    public void LevelWon()
    {
        if (_level == null)
            return;

        if (_level.LevelState < LevelState.Completed)
            _level.LevelState = LevelState.Completed;

        ColorBlock tempBlock = button.colors;
        tempBlock.normalColor = ColorForButton;
        button.colors = tempBlock;

        for (int i = 0; i < AdjecentLevels.Count; i++)
        {
            AdjecentLevels[i].UnlockLevel();
        }
    }
    public void UnlockLevel()
    {
        if (_level == null)
            return;

        if (_level.LevelState < LevelState.Unlocked)
            _level.LevelState = LevelState.Unlocked;

        button.interactable = true;
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
    public LevelState LevelState { get => _levelState; set => Setter(ref _levelState, value); }

}