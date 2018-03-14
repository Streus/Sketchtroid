using UnityEngine;
using UnityEditor;

#pragma warning disable 0168
[CustomEditor(typeof(RegisteredObject))]
public class ROProperty : Editor
{
	RegisteredObject ro;
	SerializedObject tar;

	public void OnEnable()
	{
		ro = (RegisteredObject)target;
		try
		{
			tar = new SerializedObject (ro);
		}
		catch(System.NullReferenceException nre) { }
	}

	public override void OnInspectorGUI ()
	{
		tar.Update ();

		EditorGUILayout.SelectableLabel (ro.rID, EditorStyles.largeLabel);

		GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.PrefixLabel ("Ignore Reset");
		ro.setIgnoreReset (EditorGUILayout.Toggle (ro.getIgnoreReset ()));
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.PrefixLabel ("Exclude From Directory");
		ro.setExcludeFromDirectory (EditorGUILayout.Toggle (ro.getExcludeFromDirectory ()));
		EditorGUILayout.EndHorizontal ();

		GUI.enabled = true;

		if (GUI.changed)
			EditorUtility.SetDirty (ro);
	}
}
