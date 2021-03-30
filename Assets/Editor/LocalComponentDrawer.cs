using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LocalComponentAttribute))]
public class LocalComponentDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        LocalComponentAttribute localComponentAttribute = attribute as LocalComponentAttribute;
        bool wasEnabled = GUI.enabled;
        GUI.enabled = false;
        label.text += " (local)";
        if (!localComponentAttribute.hideProperty)
            EditorGUI.PropertyField(position, property, label);
        GUI.enabled = wasEnabled;

        MonoBehaviour mono = property.serializedObject.targetObject as MonoBehaviour;
        //if(fieldInfo.FieldType.IsSubclassOf(typeof(Component)) )
        if (typeof(Component).IsAssignableFrom(fieldInfo.FieldType))
        {
            if (property.objectReferenceValue == null)
            {
                Component comp;
                if (localComponentAttribute.getComponentFromChildrens)
                    comp = mono.GetComponentInChildren(fieldInfo.FieldType);
                else
                    comp = mono.GetComponent(fieldInfo.FieldType);

                if (comp == null)
                    comp = mono.gameObject.AddComponent(fieldInfo.FieldType);

                property.objectReferenceValue = comp;
            }
        }
        else
        {
            Debug.LogError("Field <b>" + fieldInfo.Name + "</b> of " + mono.GetType() + " is not a component!", mono);
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        LocalComponentAttribute localComponentAttribute = attribute as LocalComponentAttribute;

        if (!localComponentAttribute.hideProperty)
            return EditorGUI.GetPropertyHeight(property, label);
        return -EditorGUIUtility.standardVerticalSpacing;
    }
}
