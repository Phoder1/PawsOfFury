using CustomAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
[SelectionBase]
public class BlackScreen : MonoSingleton<BlackScreen>
{
    [SerializeField]
    private float _transitionDuration;
    [SerializeField]
    private AnimationCurve _transitionInCurve;
    [SerializeField]
    private AnimationCurve _transitionOutCurve;
    public UnityEvent OnFinishTransitionIn;
    [SerializeField, LocalComponent]
    private Image _blackScreen;
    public override void OnAwake()
    {
        DontDestroyOnLoad(gameObject);
    }
    [Button]
    public void FadeIn()
    {
        _blackScreen.DOFade(1, _transitionDuration).SetEase(_transitionInCurve).OnComplete(OnComplete);

        void OnComplete() => OnFinishTransitionIn?.Invoke();
    }
    [Button]
    public void FadeOut()
    {
        _blackScreen.DOFade(0, _transitionDuration).SetEase(_transitionOutCurve);
    }
}
