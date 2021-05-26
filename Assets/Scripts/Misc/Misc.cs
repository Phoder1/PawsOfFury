using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Misc
{
    public static void Setter<T>(ref T data, T value, Action onValueChanged, Func<bool> changeCondition = null)
    {
        if ((data == null && value == null) || (data != null && data.Equals(value)))
            return;
        data = value;

        if (changeCondition == null || changeCondition())
            onValueChanged?.Invoke();
    }
}
