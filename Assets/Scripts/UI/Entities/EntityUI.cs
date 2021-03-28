using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class EntityUI : MonoBehaviour
{
    [SerializeField] HealthBar healthBar;
    [SerializeField] Image target;
    public float HealthBarHeight { get => healthBar.Height; set => healthBar.Height = value; }
    public void SetHealthBarValue(float value) => healthBar.SetValue(value);
    bool selected;
    public bool Selected
    {
        get => selected;
        set
        {
            if (selected != value)
            {
                SetTargetState(value);
                selected = value;
            }
        }
    }
    void SetTargetState(bool state)
    {
        if (!target)
            return;
        target.enabled = state;
        if (state)
            target.DOFade(0f, 1f).From(1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        else
            target.DOComplete();
    }
    private void OnDestroy()
    {
        target.DOComplete();
    }
}
