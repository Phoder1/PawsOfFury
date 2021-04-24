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
    bool inputLocked = true;
    public void SetGold(int value) => gold.text = goldPrefix + value;
}