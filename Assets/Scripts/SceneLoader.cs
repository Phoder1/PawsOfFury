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
    public void TransitionToScene(int scene)
    {
        OnTransitionStart?.Invoke();
        var _blackScreen = BlackScreen.instance;
        _blackScreen.FadeIn();
        _blackScreen.OnFinishTransitionIn.AddListener(StartSceneLoad);

        void StartSceneLoad() => GameManager.instance.StartCoroutine(LoadSceneRoutine(scene));
    }
    private IEnumerator LoadSceneRoutine(int scene)
    {
        var currentScene = SceneManager.GetActiveScene();

        OnLoadStart?.Invoke();
        yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        OnLoadFinish?.Invoke();
        yield return SceneManager.UnloadSceneAsync(currentScene);
        BlackScreen.instance.FadeOut();
    }
}