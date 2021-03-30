using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StyleAttribute))]
public class StyleDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        StyleAttribute styleAttribute = (StyleAttribute)attribute;
        AttributeStyle style = styleAttribute.style;
        bool styleValid = false;
        while (!styleValid)
            styleValid = TryStylize();

        bool TryStylize()
        {
            switch (style)
            {

                case AttributeStyle.FoldOut:
                    if (property.propertyType != SerializedPropertyType.Boolean)
                    {
                        DrawError(typeof(bool));
                        style = AttributeStyle.Default;
                        return false;
                    }
                    property.boolValue = EditorGUI.Foldout(position, property.boolValue, label);
                    break;
                case AttributeStyle.Bold:
                case AttributeStyle.Slim:
                case AttributeStyle.BoldSlim:
                    GUIStyle labelStyle;
                    switch (style)
                    {
                        case AttributeStyle.Bold:
                            labelStyle = EditorStyles.boldLabel;
                            break;
                        case AttributeStyle.Slim:
                            labelStyle = EditorStyles.miniLabel;
                            break;
                        case AttributeStyle.BoldSlim:
                            labelStyle = EditorStyles.miniBoldLabel;
                            break;
                        default:
                            labelStyle = EditorStyles.label;
                            break;
                    }
                    Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, labelStyle.CalcSize(label).y);

                    EditorGUI.LabelField(labelRect, label, labelStyle);
                    EditorGUI.PropertyField(position, property);
                    break;
                default:
                    base.OnGUI(position, property, label);
                    break;
            }
            return true;
        }
        void DrawError(Type requiredType) => Debug.LogWarning("Property was not of the required type: " + requiredType.Name + " when trying to apply the " + style.ToString() + " style.");
    }

}
