using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSaving;

[DefaultExecutionOrder(-999)]
public class GameManager : MonoSingleton<GameManager>
{
    public GameManager() : base(true) { }
    public event Action OnNewScene;

    public LinkedList<LevelSO> levelsWon = new LinkedList<LevelSO>();

    public void AddLevelWon(LevelSO level)
    {
        levelsWon.AddLast(level);
    }
    public override void OnAwake()
    {
        OnNewScene?.Invoke();
    }
    // Start is called before the first frame update
    void Start()
    {
        DOTween.defaultEaseType = Ease.Linear;
    }
    private void OnDisable()
    {
        DOTween.KillAll();
    }
    private void OnApplicationQuit()
    {
        foreach (var level in levelsWon)
            level.CompleteAndEarnReward();

        levelsWon = new LinkedList<LevelSO>();

        DataHandler.SaveAll();
    }
    public void NewSceneLoaded()
    {
        Time.timeScale = 1;
        OnNewScene?.Invoke();
    }

    private static Camera mainCam;



    public static Camera MainCam
    {
        get
        {
            if (mainCam == null)
            {
                mainCam = Camera.main;
            }
            return mainCam;
        }
    }
}
