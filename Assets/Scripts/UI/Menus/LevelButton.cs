using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class LevelButton : MonoBehaviour
{
    [SerializeField, ValueDropdown("Scenes"), InspectorName("Scene")]
    private int _sceneIndex;

    private SceneLoader _sceneLoader;

#if UNITY_EDITOR
    private ValueDropdownList<int> Scenes
    {
        get
        {
            ValueDropdownList<int> dropDownList = new ValueDropdownList<int>();
            var editorScenes = UnityEditor.EditorBuildSettings.scenes;
            for (int i = 0; i < editorScenes.Length; i++)
            {

                var path = editorScenes[i].path.Split('/');
                dropDownList.Add(new ValueDropdownItem<int>(path[path.Length - 1], i));
            }

            return dropDownList;
        }
    }
#endif
    private void Awake()
    {
        _sceneLoader = GetComponentInParent<SceneLoader>();
    }
    public void LoadScene() => _sceneLoader.TransitionToScene(_sceneIndex);
}
