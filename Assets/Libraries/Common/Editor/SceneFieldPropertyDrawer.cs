using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// https://discussions.unity.com/t/error-type-is-not-a-supported-pptr-value/817758/2
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneFieldAttribute))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);

        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.BeginChangeCheck();
        var newScene = EditorGUI.ObjectField(position, "Target Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;

        if (EditorGUI.EndChangeCheck())
        {
            var newPath = AssetDatabase.GetAssetPath(newScene);
            property.stringValue = newPath;
        }

        EditorGUI.EndProperty();
    }
}
#endif