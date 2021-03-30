using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiManager : MonoSingleton<UiManager>
{
    [SerializeField] string goldText;
    [SerializeField] TextMeshProUGUI gold;
    public void SetGold(int value) => gold.text = goldText + value;
    public override void Awake()
    {
        base.Awake();
        BlackBoard.uiManager = _instance;
    }
}
