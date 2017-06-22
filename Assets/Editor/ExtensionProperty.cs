using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Extension))]
public class ExtensionProperty : PropertyDrawer
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty (position, label, property);

		EditorGUILayout.PropertyField (property.FindPropertyRelative ("inst"));

		EditorGUI.EndProperty ();
	}
}
