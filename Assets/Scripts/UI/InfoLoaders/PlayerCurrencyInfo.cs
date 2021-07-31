using DataSaving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCurrencyInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _crystals;
    [SerializeField]
    private TextMeshProUGUI _monsterGoo;
    public void Load(PlayerCurrency playerCurrency)
    {
        if (_crystals != null)
            _crystals.text = playerCurrency.Crystals.ToString();

        if (_monsterGoo != null)
            _monsterGoo.text = playerCurrency.MonsterGoo.ToString();
    }
}
