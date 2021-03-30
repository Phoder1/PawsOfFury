using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RequiredFieldAttribute))]
public class RequiredFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RequiredFieldAttribute requiredFieldAttribute = (RequiredFieldAttribute)attribute;
        if(property.objectReferenceValue == null)
        {
            Color previousColor = GUI.color;
            string errorMessage = "Field " + property.name + " is required!";
            switch (requiredFieldAttribute.errorLogType)
            {
                case ErrorLogType.Warning:
                    GUI.color = Color.yellow;
                    break;
                case ErrorLogType.Error:
                    GUI.color = Color.red;
                    break;
                default:
                    GUI.color = Color.gray;
                    break;
            }
            EditorGUI.PropertyField(position, property, label);
            GUI.color = previousColor;
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }

    }
}
