using UnityEngine;

public class RenameAttribute : PropertyAttribute
{
    private string label;
    public string Label => label;
    public RenameAttribute(string label)
    {
        this.label = label;
    }

}
