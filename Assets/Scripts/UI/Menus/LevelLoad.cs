using Sirenix.OdinInspector;
using UnityEngine;

[HideMonoScript]
public class LevelLoad : MonoBehaviour
{
    [SerializeField, HideIf("@_restartLevel")]
    private LevelSO _level;
    [SerializeField]
    private bool _restartLevel = false;
    public LevelSO Level => _level;

    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = GetComponentInParent<SceneLoader>();
    }
    public void LoadScene()
    {
        if (_restartLevel && LevelManager.instance != null)
            LoadScene(LevelManager.instance.Level);
        else if (Level != null)
            LoadScene(Level);
    }
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
