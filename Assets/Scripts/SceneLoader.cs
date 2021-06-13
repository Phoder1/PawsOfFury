using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnLoadStart;
    [SerializeField]
    private UnityEvent OnLoadFinish;

    public async void LoadSceneAsset(AssetReference sceneAsset)
    {
        var scene = await sceneAsset.LoadSceneAsync(LoadSceneMode.Additive, false).Task;
    }
}