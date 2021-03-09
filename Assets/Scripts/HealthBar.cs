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
    public float timeUntilDisapear;
    const float minFadeDuration = 0.5f;

    Image[] healthBarImages;
    [HideInInspector] public Vector3 healthbarHeight;
    private Canvas canvas;
    Coroutine UpdateDamageFillCoro;
    public void Init()
    {
        canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        transform.SetParent(canvas.transform);
        healthBarImages = GetComponentsInChildren<Image>();
    }
    public void FadeInOut(float alpha, float duration)
    {
        duration = Mathf.Max(duration, minFadeDuration);
        foreach (Image image in healthBarImages)
            image.DOFade(alpha, duration);

    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
    public void SetValue(float value)
    {
        bool destroyed = value == 0;
        if (fill.fillAmount >= value)
        {
            if (UpdateDamageFillCoro != null)
                StopCoroutine(UpdateDamageFillCoro);
            if (!destroyed)
                UpdateDamageFillCoro = StartCoroutine(UpdateDamageFill());
        }
        else if (damageFill.fillAmount < value)
        {
            if (UpdateDamageFillCoro != null)
                StopCoroutine(UpdateDamageFillCoro);
            //damageFill.DOKill();
            damageFill.DOFillAmount(value, 0.2f);
        }
        if(!destroyed)
        fill.DOFillAmount(value, 0.2f);
        else
        fill.DOFillAmount(value, 0.2f).OnComplete(() => { Destroy(gameObject); });
        //Todo: Fadeout then destroy no damage fill
        //Todo: instant damage fill update on heal
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
}
