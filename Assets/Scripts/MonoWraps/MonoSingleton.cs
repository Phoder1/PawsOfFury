using UnityEngine;
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T instance;

    public void Awake()
    {
        if (isActiveAndEnabled)
        {
            if (instance == null)
                instance = this as T;
            else if (instance != this as T)
                Destroy(gameObject);
        }
        OnAwake();
    }
    public virtual void OnAwake() { }
}
