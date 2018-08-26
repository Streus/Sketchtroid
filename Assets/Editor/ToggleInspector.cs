using UnityEngine;
using UnityEditor;
using CircuitNodes;

[CustomEditor(typeof(Toggle))]
public class ToggleInspector : Editor
{
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		EditorGUILayout.Separator ();

		Toggle t = (Toggle)target;
		if (GUILayout.Button ("Toggle"))
			t.SetActive (!t.IsActivated ());
	}
}
