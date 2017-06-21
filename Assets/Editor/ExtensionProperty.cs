using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Extension))]
public class ExtensionProperty : PropertyDrawer
{
	GameObject go;

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty (position, label, property);

		Rect goField = new Rect (position.x, position.y, position.width, position.height);
		EditorGUI.ObjectField (goField, "Object", go, typeof(GameObject), true);

		EditorGUI.EndProperty ();
	}
}
