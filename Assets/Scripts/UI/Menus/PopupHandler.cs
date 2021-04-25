using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    [SerializeField]
    private Image panel;
    [SerializeField]
    private float fadeInDuration;
    [SerializeField]
    private GameObject popupWindow;
    private void OnEnable()
    {
        Init();
    }
    public void Init()
    {
        panel.DOFade(panel.color.a, fadeInDuration).From(0);
        popupWindow.transform.DOScale(1, fadeInDuration).From(0);
    }
}
