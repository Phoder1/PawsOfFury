using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockPlaceablePath : MonoBehaviour
{
    [SerializeField]
    private LayerMask _newLayer;
    [SerializeField]
    private Renderer _renderer;

    private bool _unlocked = false;
    public void Unlock()
    {
        if (_unlocked)
            return;

        gameObject.layer = _newLayer;
        _renderer.material.EnableKeyword("_EMISSION");
        _unlocked = true;
    }
}
