using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHandler : MonoSingleton<CoroutineHandler>
{
    public CoroutineHandler() : base(false) { }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    public override void OnAwake() => IngameBlackBoard.coroutineHandler = instance;
}
