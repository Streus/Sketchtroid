using UnityEngine;
using UnityEditor;

#pragma warning disable 0168
[CustomEditor(typeof(RegisteredObject))]
public class ROProperty : Editor
{
	RegisteredObject ro;
	SerializedObject tar;
	//SerializedProperty uID;

	public void OnEnable()
	{
		ro = (RegisteredObject)target;
		try
		{
			tar = new SerializedObject (ro);
		}
		catch(System.NullReferenceException nre) { }
		//uID = target.FindProperty ("rID");
	}

	public override void OnInspectorGUI ()
	{
		tar.Update ();

		EditorGUILayout.LabelField ("ID: " + ro.rID.ToString("0000000000"));

		if (GUI.changed)
			EditorUtility.SetDirty (ro);
	}
}
