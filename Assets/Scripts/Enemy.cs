using UnityEngine;
using static EntityStats;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject UiObject;
    [Tooltip("Height 0 is the center of the Unit.")]
    [SerializeField] float healthbarHeight;
    [SerializeField] DefualtStats defualtStats;
    Camera mainCam;
    HealthBar healthBar;
    public EntityStats stats;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        UiObject = Instantiate(UiObject, mainCam.WorldToScreenPoint(transform.position) + Vector3.up * healthbarHeight, transform.rotation);
        healthBar = UiObject.GetComponent<HealthBar>();
        healthBar.Init();
        LevelManager._instance.AddToList(this);
        stats = new EntityStats();
        FillDictionary();
    }
    private void FillDictionary()
    {
        Stat maxHp = new Stat(this, StatType.MaxHP, defualtStats.MaxHP);

        stats.Add(maxHp);
        stats.Add(new HpStat(this, StatType.HP, defualtStats.HP, healthBar, maxHp, new
            Reaction[1] {
            new Reaction(new Death(),
            (value) => {Destroy(); })
            }
            ));
        stats.Add(new Stat(this, StatType.Damage, defualtStats.Damage));
        stats.Add(new Stat(this, StatType.AttackSpeed, defualtStats.AttackSpeed));
        stats.Add(new Stat(this, StatType.Range, defualtStats.Range));
    }
    public void Destroy()
    {
        LevelManager._instance.RemoveFromList(this);
        Destroy(gameObject);
    }
}
