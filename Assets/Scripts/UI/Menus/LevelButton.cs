//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AddressableAssets;

//public class LevelButton : MonoBehaviour
//{
//    [SerializeField]
//    private AssetReference _levelRefrence;

//    private SceneLoader sceneLoader;

//    private void Awake()
//    {
//        sceneLoader = GetComponentInParent<SceneLoader>();
//    }
//  //  public void LoadScene() => sceneLoader.LoadScene(_levelRefrence);
//    private void OnValidate()
//    {
//        Debug.Log(_levelRefrence.Asset?.GetType());
//    }
//}
