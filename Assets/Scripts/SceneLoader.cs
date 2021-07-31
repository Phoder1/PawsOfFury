using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnTransitionStart;
    [SerializeField]
    private UnityEvent OnLoadStart;
    [SerializeField]
    private UnityEvent OnLoadFinish;

    private bool _loadingScene = false;
    private string _sceneToLoad = string.Empty;
    private void OnEnable()
    {
        BlackScreen.instance.OnFinishTransitionIn.AddListener(StartSceneLoad);
    }
    private void OnDisable()
    {
        BlackScreen.instance.OnFinishTransitionIn.RemoveListener(StartSceneLoad);
    }
    public void TransitionToScene(string scene)
    {
        if (_loadingScene)
            return;

        _loadingScene = true;
        _sceneToLoad = scene;

        OnTransitionStart?.Invoke();
        var _blackScreen = BlackScreen.instance;
        
        if (_blackScreen != null)
            _blackScreen.FadeIn();
        else
            StartSceneLoad();
    }
    public void StartSceneLoad()
    {
        if (string.IsNullOrEmpty(_sceneToLoad))
            return;

        GameManager.instance.StartCoroutine(LoadSceneRoutine(_sceneToLoad));
    }
    private IEnumerator LoadSceneRoutine(string scene)
    {
        OnLoadStart?.Invoke();
        yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        OnLoadFinish?.Invoke();

        if (BlackScreen.instance != null)
            BlackScreen.instance.FadeOut();

        GameManager.instance.NewSceneLoaded();
    }
}