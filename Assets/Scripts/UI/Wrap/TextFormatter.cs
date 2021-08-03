using Sirenix.OdinInspector;
using System;
using UnityEngine;

[HelpURL("https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings")]
[DefaultExecutionOrder(-1)]
public class TextFormatter : BaseFormatter<string>
{
    [SerializeField]
    private string prefix = default;
    [SerializeField]
    private string suffix = default;
    [SerializeField, Tooltip("A string used in the ToString() method, go to the help URL for more info.")]
    private string _toStringFormat = default;
    [SerializeField, FoldoutGroup("String Overrides")]
    private StringOverrides[] _stringOverrides = default;
    public void Format(int value)
        => Format(value.ToString(_toStringFormat));
    public void Format(float value)
        => Format(value.ToString(_toStringFormat));
    public void Format(byte value)
        => Format(value.ToString(_toStringFormat));
    protected override string FormatInput(string stringValue)
    {
        Array.ForEach(_stringOverrides, (x) => stringValue = x.Format(stringValue));
        stringValue = prefix + stringValue + suffix;
        return stringValue;
    }
    [Serializable]
    private class StringOverrides
    {
        public bool exactMatch = false;
        public string value = default;
        public string overrideValue = default;
        public string Format(string text)
        {
            if (exactMatch)
            {
                if (text.Trim() == value)
                    return overrideValue;
            }
            else
            {
                return text.Replace(value, overrideValue);
            }

            return text;
        }
    }

}