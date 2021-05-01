using CustomAttributes;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour, IPointerUpHandler
{
    public UnitSO unit;
    [SerializeField, LocalComponent]
    private DataImporter _dataImporter;
    [SerializeField]
    private UnityEvent<object> OnClickEvent;

    public void OnPointerUp(PointerEventData eventData)
    {
        OnClickEvent?.Invoke(unit);
    }
    private void Start()
    {
        _dataImporter.Import(unit);
    }
}
