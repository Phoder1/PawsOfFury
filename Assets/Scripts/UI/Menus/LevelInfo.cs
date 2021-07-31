using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class LevelInfo : MonoBehaviour
{
    [SerializeField, FoldoutGroup("Refrences")]
    private TextMeshProUGUI _crystalsReward;
    public void Load(LevelSO level)
    {
        if (_crystalsReward != null)
            _crystalsReward.text = level.CrystalsReward.ToString();
    }
}
