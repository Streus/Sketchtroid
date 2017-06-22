using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RegisteredObject))]
public class ROProperty : Editor
{
	RegisteredObject ro;
	SerializedObject tar;
	//SerializedProperty uID;

	public void OnEnable()
	{
		ro = (RegisteredObject)target;
		tar = new SerializedObject (ro);
		//uID = target.FindProperty ("rID");
	}

	public override void OnInspectorGUI ()
	{
		tar.Update ();

		EditorGUILayout.LabelField ("ID: " + ro.rID.ToString("0000000000"));
	}
}