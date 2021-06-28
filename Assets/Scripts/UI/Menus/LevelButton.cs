using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using System;

public class LevelButton : MonoBehaviour
{
    [SerializeField, ValueDropdown("Scenes"), InspectorName("Scene")]
    private string _sceneName;

    private SceneLoader _sceneLoader;

#if UNITY_EDITOR
    private string[] Scenes
    {
        get
        {
            var scenes = UnityEditor.EditorBuildSettings.scenes;
            string[] sceneNames = new string[scenes.Length];

            for (int i = 0; i < scenes.Length; i++)
                sceneNames[i] = GetFileName(scenes[i].path);

            return sceneNames;
        }
    }
    private string GetFileName(string path)
    {
        var splitPath = path.Split('/');
        splitPath = splitPath[splitPath.Length - 1].Split('.');
        return splitPath[splitPath.Length - 2];
    }
#endif
    private void Awake()
    {
        _sceneLoader = GetComponentInParent<SceneLoader>();
    }
    public void LoadScene() => _sceneLoader.TransitionToScene(_sceneName);
}
