using UnityEngine;
public enum AttributeStyle { Default, FoldOut, Bold, Slim, BoldSlim}
public class StyleAttribute : PropertyAttribute
{
    public readonly AttributeStyle style;
    public StyleAttribute(AttributeStyle style)
    {
        this.style = style;
    }
}
