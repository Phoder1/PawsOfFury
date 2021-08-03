using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using System;

[HideMonoScript]
public class LevelLoad : MonoBehaviour
{
    [SerializeField]
    private LevelSO _level;
    public LevelSO Level => _level;

    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = GetComponentInParent<SceneLoader>();
    }
    public void LoadScene() => LoadScene(Level);
    public void LoadScene(LevelSO level)
    {
        if (_sceneLoader == null || level == null)
            return;

        _sceneLoader.TransitionToScene(level.SceneName);
    }
    public void LoadSelectedLevel()
    {
        if (LevelSelection.SelectedLevel != null)
            LoadScene(LevelSelection.SelectedLevel);
    }
}
