using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EffectDataSO))]
public class EffectDataSoEditor : Editor
{
    SerializedProperty targetsProperty;
    SerializedProperty affectedStatProperty;
    SerializedProperty effectTypeProperty;
    SerializedProperty inPrecentageProperty;
    SerializedProperty isRelativeToMaxProperty;
    SerializedProperty amountProperty;
    SerializedProperty durationProperty;
    SerializedProperty tickTimeProperty;
    private void OnEnable()
    {
        targetsProperty = serializedObject.FindProperty("targets");
        affectedStatProperty = serializedObject.FindProperty("affectedStat");
        effectTypeProperty = serializedObject.FindProperty("effectType");
        inPrecentageProperty = serializedObject.FindProperty("inPercentage");
        isRelativeToMaxProperty = serializedObject.FindProperty("isRelativeToMax");
        amountProperty = serializedObject.FindProperty("amount");
        durationProperty = serializedObject.FindProperty("duration");
        tickTimeProperty = serializedObject.FindProperty("tickTime");
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUILayout.PropertyField(targetsProperty);
        EditorGUILayout.PropertyField(affectedStatProperty);
        EditorGUILayout.PropertyField(effectTypeProperty);
        EditorGUILayout.PropertyField(amountProperty);
        EditorGUILayout.PropertyField(inPrecentageProperty);
        EditorGUILayout.PropertyField(isRelativeToMaxProperty);
        if (inPrecentageProperty.boolValue)
            serializedObject.ApplyModifiedProperties();


        switch ((EffectType)effectTypeProperty.enumValueIndex)
        {
            case EffectType.Instant:
                break;
            case EffectType.Toggle:
                EditorGUILayout.PropertyField(durationProperty);
                break;
            case EffectType.OverTime:
                EditorGUILayout.PropertyField(durationProperty);
                EditorGUILayout.PropertyField(tickTimeProperty);
                EditorGUILayout.LabelField("Rounded duration: " + Mathf.Floor(durationProperty.floatValue / tickTimeProperty.floatValue) * tickTimeProperty.floatValue);
                string DPS;
                if (inPrecentageProperty.boolValue)
                    if (isRelativeToMaxProperty.boolValue)
                        DPS = (amountProperty.floatValue / tickTimeProperty.floatValue).ToString("0.0") + "% from max stat value.";
                    else
                        DPS = ((Mathf.Pow(1 + (amountProperty.floatValue / 100), 1 / tickTimeProperty.floatValue) - 1) * 100).ToString("0.0") + "% from current stat value.";
                else
                    DPS = (amountProperty.floatValue / tickTimeProperty.floatValue).ToString("0.0");

                if (amountProperty.floatValue <= -100 && inPrecentageProperty.boolValue)
                    EditorGUILayout.LabelField("Amount per seconds: will die on 1st tick, please switch to instant if that's the desired outcome.");
                else
                    EditorGUILayout.LabelField("Amount per seconds: " + DPS);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
