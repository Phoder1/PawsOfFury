using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public event Action OnNewScene;
    public override void OnAwake()
    {
        DontDestroyOnLoad(gameObject);
        OnNewScene?.Invoke();
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
    public void NewSceneLoaded() => OnNewScene?.Invoke();
}
