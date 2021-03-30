using UnityEngine;

public class LocalComponentAttribute : PropertyAttribute
{
    public readonly bool getComponentFromChildrens;
    public readonly bool hideProperty;

    public LocalComponentAttribute(bool hideProperty = false, bool getComponentFromChildrens = false)
    {
        this.getComponentFromChildrens = getComponentFromChildrens;
        this.hideProperty = hideProperty;
    }
}