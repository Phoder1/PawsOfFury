using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelVictory : MonoBehaviour
{
    [SerializeField]
    private LevelSO _level;
    [SerializeField]
    private UnityEvent<LevelSO> OnLevelComplete;

    public void LevelVectory() => OnLevelComplete?.Invoke(_level);
}
