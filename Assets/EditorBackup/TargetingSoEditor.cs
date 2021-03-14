//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(TargetingSO))]
//public class TargetingSoEditor : Editor
//{
//    SerializedProperty targetsProperty;
//    SerializedProperty priorityProperty;
//    SerializedProperty lockingProperty;

//    int targetArraySize;
//    private void OnEnable()
//    {
//        targetsProperty = serializedObject.FindProperty("targets");
//        priorityProperty = serializedObject.FindProperty("priorities");
//        lockingProperty = serializedObject.FindProperty("locking");
//        //targetArraySize = targetsProperty.arraySize;
//    }
//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        targetArraySize = EditorGUILayout.IntField("Possible Targets", targetsProperty.arraySize);
//        if (targetsProperty.arraySize != targetArraySize)
//            targetsProperty.arraySize = targetArraySize;
//        if (priorityProperty.arraySize != targetArraySize)
//            priorityProperty.arraySize = targetArraySize;
//        serializedObject.ApplyModifiedProperties();

//        for (int i = 0; i < targetArraySize; i++)
//        {
//            EditorGUILayout.BeginHorizontal();
//            EditorGUILayout.PropertyField(targetsProperty.GetArrayElementAtIndex(i), GUIContent.none);
//            EditorGUILayout.PropertyField(priorityProperty.GetArrayElementAtIndex(i), GUIContent.none);
//            EditorGUILayout.EndHorizontal();
//        }
//        EditorGUILayout.PropertyField(lockingProperty);
//        serializedObject.ApplyModifiedProperties();
//    }
//}
