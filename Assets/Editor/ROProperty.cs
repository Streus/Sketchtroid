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

		EditorGUILayout.SelectableLabel (ro.RID, EditorStyles.largeLabel);

		GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.PrefixLabel ("Ignore Reset");
		ro.SetIgnoreReset (EditorGUILayout.Toggle (ro.GetIgnoreReset ()));
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.PrefixLabel ("Exclude From Directory");
		ro.SetExcludeFromDirectory (EditorGUILayout.Toggle (ro.GetExcludeFromDirectory ()));
		EditorGUILayout.EndHorizontal ();

		GUI.enabled = true;

		if (GUI.changed)
			EditorUtility.SetDirty (ro);
	}
}
