using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrystalsMonitor : MonoBehaviour
{
    public List<GameObject> crystals;
    [HideInInspector]
    public List<TextMeshProUGUI> texts;

    private void Awake()
    {
        if (texts == null)
            texts = crystals.ConvertAll((x) => x.GetComponentInChildren<TextMeshProUGUI>());
    }
}
