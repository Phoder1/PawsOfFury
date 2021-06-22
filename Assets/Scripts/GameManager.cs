using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public override void OnAwake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        DOTween.defaultEaseType = Ease.Linear;
    }
    private void OnDisable()
    {
        DOTween.KillAll();
    }
}
