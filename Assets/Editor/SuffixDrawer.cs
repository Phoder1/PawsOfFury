using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(SuffixAttribute))]
public class SuffixDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SuffixAttribute suffixAttribute = (SuffixAttribute)attribute;
        if (suffixAttribute.suffix == "")
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }
        GUIContent suffixContent = new GUIContent(suffixAttribute.suffix);
        Vector2 suffixSize = EditorStyles.label.CalcSize(suffixContent);
        float suffixWidth = suffixSize.x + 2 * suffixAttribute.padding;
        position.width -= suffixWidth;
        EditorGUI.PropertyField(position, property, label);
        Rect suffixRect = new Rect(position.width + position.x + suffixAttribute.padding, position.y, suffixSize.x + suffixAttribute.padding, position.height);
        EditorGUI.LabelField(suffixRect, suffixContent);
    }
}
