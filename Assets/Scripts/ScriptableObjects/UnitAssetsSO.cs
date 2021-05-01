using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UnitAssets")]
public class UnitAssetsSO : ScriptableObject
{
    [SerializeField]
    private Sprite[] _tierCrystals;
    public Sprite[] TierCrystals => _tierCrystals;
}
