using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FlagsHelper
{
    public static int SortingLayerToLayerMask(this int sortingLayer) => (int)Mathf.Pow(2, sortingLayer);
    public static bool IsSet(this LayerMask layerMask, int layer) => (layerMask.value & layer) != 0;

    public static void SetValue(this ref LayerMask layerMask, int layer, bool value)
    {
        if (value)
            layerMask.Set(layer);
        else
            layerMask.Unset(layer);
    }
    public static void Set(this ref LayerMask layerMask, int layer) => layerMask |= layer;

    public static void Unset(this ref LayerMask layerMask, int layer) => layerMask &= (~layer);
    public static void Flip(this ref LayerMask layerMask) => layerMask = ~layerMask ^ ~0;
}
