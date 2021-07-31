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
    public void TransitionToScene(string scene)
    {
        if (_loadingScene)
            return;

        _loadingScene = true;

        OnTransitionStart?.Invoke();
        var _blackScreen = BlackScreen.instance;
        if (_blackScreen != null)
        {
            _blackScreen.FadeIn();
            _blackScreen.OnFinishTransitionIn.AddListener(StartSceneLoad);
        }
        else
            StartSceneLoad();

        void StartSceneLoad() => GameManager.instance.StartCoroutine(LoadSceneRoutine(scene));
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