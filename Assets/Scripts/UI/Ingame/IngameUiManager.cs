using Assets.StateMachine;
using CustomAttributes;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class IngameUiManager : MonoSingleton<IngameUiManager>
{
    [SerializeField] string goldPrefix;
    [SerializeField] TextMeshProUGUI gold;

    public IngameUiManager() : base(false)
    {
    }

    public void SetGold(int value) => gold.text = goldPrefix + value;
}