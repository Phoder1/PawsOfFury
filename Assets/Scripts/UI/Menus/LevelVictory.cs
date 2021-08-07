using DataSaving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelVictory : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<LevelSO> OnLevelComplete;
    [SerializeField]
    private UnityEvent<UnitSO> OnUnitWon;

    public void LevelVectory()
    {
        var levelManager = LevelManager.instance;

        if (levelManager == null || levelManager.Level == null)
            return;

        //GameManager.instance.AddLevelWon(levelManager.Level);
        var reward = levelManager.Level.CompleteAndEarnReward();
        
        if (reward.unit != null)
            OnUnitWon?.Invoke(reward.unit);

        OnLevelComplete?.Invoke(levelManager.Level);
        DataHandler.SaveAll();
    }
}
