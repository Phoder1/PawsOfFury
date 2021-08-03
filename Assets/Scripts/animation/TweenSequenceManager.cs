using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(1)]
public class TweenSequenceManager : MonoBehaviour
{
    [SerializeField]
    private List<SequnceTween> _tweenAnimations = default;

    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnComplete = default;
    [SerializeField, FoldoutGroup("Events")]
    private UnityEvent OnRewind = default;
    Sequence _sequence = default;
    private void Start()
    {
        _sequence = DOTween.Sequence();
        _sequence.SetAutoKill(false);
        _sequence.onRewind += OnWindowClose;
        _sequence.onComplete += OnWindowOpen;
        _sequence.Pause();
        foreach (var tweenAnimation in _tweenAnimations)
        {
            Tween tween = tweenAnimation.tweenAnimation.tween;
            if (tween != null)
                _sequence.Insert(tweenAnimation.delay, tween);
        }
    }
    private void OnDestroy()
    {
        if (_sequence.IsActive())
            _sequence.Kill();
    }
    public void PlayForward()
    {
        _sequence.PlayForward();
    }
    public void PlayBackwards()
    {
        _sequence.PlayBackwards();
    }
    public void Pause()
    {
        _sequence.Pause();
    }
    public void Play()
    {
        _sequence.Play();
    }
    void OnWindowOpen()
    {
        OnComplete?.Invoke();
    }
    void OnWindowClose()
    {
        OnRewind?.Invoke();
    }

    [Serializable, InlineProperty]
    private class SequnceTween
    {
        public DOTweenAnimation tweenAnimation = null;
        public float delay = 0;
    }
}
