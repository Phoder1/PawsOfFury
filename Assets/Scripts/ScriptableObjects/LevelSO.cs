using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = Database.SODataFol + "Level")]
[InlineEditor]
public class LevelSO : ScriptableObject
{
    [SerializeField, ValueDropdown("Scenes"), InspectorName("Scene")]
    private string _sceneName;
    [BoxGroup("Rewards")]
    [SerializeField, Range(0,1)]
    private float _chanceToGetACard;
    [BoxGroup("Rewards")]
    [SerializeField]
    private int _crystalsReward = 0;

    public string SceneName => _sceneName;
    public float ChanceToGetACard => _chanceToGetACard;
    public int CrystalsReward => _crystalsReward;

    public void EarnedReward()
    {

    }
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
}
