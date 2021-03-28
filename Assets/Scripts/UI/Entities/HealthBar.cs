using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] Image fill;
    [SerializeField] Image damageFill;
    [Tooltip("Delay until damage fill is also affected.")]
    [SerializeField] float damageFillDelay;

    [Tooltip("Amount of time of not taking/recieving HP until bar starts disapearing.")]
    [SerializeField] float timeUntilDisapear;
    const float minFadeDuration = 0.5f;

    Image[] healthBarImages;
    [HideInInspector] public Vector3 healthbarHeight;
    Coroutine updateDamageFillCoro;
    Coroutine hideHealthBarCoro;
    bool healthBarShown = false;
    float height = 0;

    public float Height
    {
        get => height;
        set
        {
            height = value;
            transform.localPosition = Vector3.up * height;
        }
    }
    private void Start()
    {
        healthBarImages = GetComponentsInChildren<Image>();
    }
    void FadeInOut(float alpha, float duration)
    {
        duration = Mathf.Max(duration, minFadeDuration);
        foreach (Image image in healthBarImages)
        {
            image.DOComplete();
            image.DOFade(alpha, duration);
        }
    }
    void ShowHealthbar()
    {
        if (!healthBarShown)
        {
            FadeInOut(1, 0.5f);
            healthBarShown = true;
        }
    }
    public void SetValue(float value)
    {
        bool destroyed = value == 0;
        if (fill.fillAmount != value)
        {
            if (updateDamageFillCoro != null)
                StopAndNullCoroutine(ref updateDamageFillCoro);
            if (hideHealthBarCoro != null)
                StopAndNullCoroutine(ref hideHealthBarCoro);
            hideHealthBarCoro = StartCoroutine(HideHealthBar());
            if (!healthBarShown)
                ShowHealthbar();

        }
        if (fill.fillAmount > value)
        {
            if (!destroyed)
                updateDamageFillCoro = StartCoroutine(UpdateDamageFill());
        }
        else if (damageFill.fillAmount < value)
        {
            damageFill.DOComplete();
            damageFill.DOFillAmount(value, 0.2f);
        }
        fill.DOComplete();

        if (!destroyed)
            //    fill.fillAmount = value;
            fill.DOFillAmount(value, 0.2f);
        else
            //    Destroy(gameObject);
            fill.DOFillAmount(value, 0.2f).OnComplete(() => Destroy(transform.parent.gameObject));
        //Todo: Fadeout then destroy no damage fill
        //Todo: instant damage fill update on heal
    }

    void StopAndNullCoroutine(ref Coroutine coroutine)
    {
        StopCoroutine(coroutine);
        coroutine = null;
    }

    IEnumerator UpdateDamageFill()
    {
        yield return new WaitForSeconds(damageFillDelay);
        if (damageFill.fillAmount > fill.fillAmount)
        {
            //damageFill.DOKill();
            damageFill.DOFillAmount(fill.fillAmount, 0.5f);
        }
    }
    IEnumerator HideHealthBar()
    {
        yield return new WaitForSeconds(timeUntilDisapear);
        if (healthBarShown)
        {
            FadeInOut(0, 0.5f);
            healthBarShown = false;
        }
        hideHealthBarCoro = null;
    }

    private void OnDestroy()
    {
        foreach (Image image in healthBarImages)
            image.DOComplete();
        StopAllCoroutines();
    }
}
