using DG.Tweening;
using UnityEngine;

public class Pauser : MonoBehaviour
{
    [SerializeField]
    private float _pauseTweenDuration = 0.4f;

    private bool _paused = false;
    private Tween _pauseTween = null;
    private void Awake()
    {
        _pauseTween = TweenTimeScale(0, _pauseTweenDuration).SetAutoKill(false).Pause().SetUpdate(true);
    }
    private void OnDestroy()
    {
        if (_pauseTween.IsActive())
            _pauseTween.Kill();
    }
    public void Pause()
    {
        _pauseTween.PlayForward();
        _paused = true;
    }
    public void Resume()
    {
        _pauseTween.PlayBackwards();
        _paused = false;
    }
    public void TogglePause()
    {
        if (_paused)
        {
            _pauseTween.PlayBackwards();
        }
        else
        {
            _pauseTween.PlayForward();
        }

        _paused = !_paused;

    }
    private static Tween TweenTimeScale(float to, float duration, Ease ease = Ease.Linear) => DOTween.To(() => Time.timeScale, (x) => Time.timeScale = x, to, duration).SetEase(ease);
}
