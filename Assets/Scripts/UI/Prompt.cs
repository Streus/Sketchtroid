using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Prompt : MonoBehaviour
{
	/* Static Vars */


	#region INSTANCE_VARS

	[SerializeField]
	private Text description;
	[SerializeField]
	private Transform optionList;
	#endregion

	#region STATIC_METHODS

	public static Prompt Create (RectTransform parent, string description, params Option[] options)
	{
		GameObject pref = ABU.LoadAsset<GameObject> ("core", "Prompt");
		GameObject inst = Instantiate (pref, parent, false);
		Prompt p = inst.GetComponent<Prompt> ();

		p.description.text = description;

		foreach (Option o in options)
			p.AddOption (o);

		return p;
	}
	#endregion


	#region INSTANCE_METHODS

	public void AddOption (Option option)
	{
		GameObject butPref = ABU.LoadAsset<GameObject> ("core", "TempButton"); //TODO replace with final button version
		GameObject inst = Instantiate(butPref, optionList, false);
		inst.transform.GetChild (0).GetComponent<Text> ().text = option.text;
		if (option.function == null)
			option.function = DestroySelf;
		inst.GetComponent<Button> ().onClick.AddListener (option.function);
	}

	private void DestroySelf()
	{
		Destroy (gameObject);
	}
	#endregion

	#region INTERNAL_TYPES

	// An inner struct that pairs option text with a function
	public struct Option
	{
		public string text;
		public UnityAction function;

		public Option(string text, UnityAction function)
		{
			this.text = text;
			this.function = function;
		}
	}
	#endregion
}
