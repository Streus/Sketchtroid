﻿using UnityEngine;
using UnityEditor;
using CircuitNodes;

[CustomEditor(typeof(CircuitNodes.Toggle))]
public class ToggleInspector : Editor
{
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();
		Toggle t = (Toggle)target;
		if (GUILayout.Button ("Toggle"))
			t.setActive (!t.isActivated ());
	}
}