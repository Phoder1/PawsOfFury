using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RenameAttribute))]
public class RenameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RenameAttribute labelAttribute = attribute as RenameAttribute;
        label.text = labelAttribute.Label;
        EditorGUI.PropertyField(position, property, label);
    }
}
