using UnityEngine;
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    protected static T _instance;

    public void Awake()
    {
        if (isActiveAndEnabled)
        {
            if (_instance == null)
                _instance = this as T;
            else if (_instance != this as T)
                Destroy(this);
        }
        OnAwake();
    }
    public virtual void OnAwake() { }
}
