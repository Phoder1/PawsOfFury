﻿using UnityEngine;
public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
{
    protected static T _instance;

    public virtual void Awake() {
        if (isActiveAndEnabled) {
            if (_instance == null)
                _instance = this as T;
            else if (_instance != this as T)
                Destroy(this);
        }
    }
}
