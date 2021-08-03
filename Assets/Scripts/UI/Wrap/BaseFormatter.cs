using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public abstract class BaseFormatter<T> : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<T> OnFormat = default;

    public void Format(T input)
    {
        input = FormatInput(input);
        OnFormat?.Invoke(input);
    }

    protected virtual T FormatInput(T input) => input;
}
