using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelInfo : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Refrences")]
    private UnityEvent<string> _name;
    [SerializeField, FoldoutGroup("Refrences")]
    private UnityEvent<float> _chanceToGetACard;
    [SerializeField, FoldoutGroup("Refrences")]
    private UnityEvent<int> _crystalReward;
    public void Load(LevelSO levelSO)
    {
        if (levelSO == null)
            return;

        _name?.Invoke(levelSO.LevelName);
        _chanceToGetACard?.Invoke(levelSO.ChanceToGetACard);
        _crystalReward?.Invoke(levelSO.CrystalsReward);
    }
}
