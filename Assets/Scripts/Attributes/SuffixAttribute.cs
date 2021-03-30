using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SuffixAttribute : PropertyAttribute
{
    public readonly string suffix;
    public readonly float padding;

    public SuffixAttribute(string suffix, float padding = 3)
    {
        this.suffix = suffix;
        this.padding = padding;
    }
}
