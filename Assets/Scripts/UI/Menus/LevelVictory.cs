using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelVictory : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<LevelSO> OnLevelComplete;

    public void LevelVectory()
    {
        var levelManager = LevelManager.instance;

        if (levelManager == null || levelManager.Level == null)
            return;

        GameManager.instance.AddLevelWon(levelManager.Level);
        OnLevelComplete?.Invoke(levelManager.Level);
    }
}
