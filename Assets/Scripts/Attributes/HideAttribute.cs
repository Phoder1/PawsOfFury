using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class HideAttribute : PropertyAttribute
{
    //The name of the bool field that will be in control
    public readonly string ConditionalSourceField;
    //TRUE = Hide in inspector / FALSE = Disable in inspector 
    public readonly bool HideInInspector;
    public readonly int indentLevel;
    public HideAttribute(string conditionalSourceField, bool hideInInspector = false, int indentLevel = 0)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
        this.indentLevel = indentLevel;
    }
}