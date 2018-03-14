using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Prompt : MonoBehaviour
{
	/* Static Vars */


	/* Instance Vars */
	[SerializeField]
	private Text description;
	[SerializeField]
	private Transform optionList;

	/* Static Methods */
	public static Prompt create (RectTransform parent, string description, params Option[] options)
	{
		GameObject pref = AssetBundleUtil.loadAsset<GameObject> ("core/prefabs/ui", "Prompt");
		GameObject inst = Instantiate (pref, parent, false);
		Prompt p = inst.GetComponent<Prompt> ();

		p.description.text = description;

		foreach (Option o in options)
			p.addOption (o);

		return p;
	}

	/* Instance Methods */
	public void addOption (Option option)
	{
		GameObject butPref = AssetBundleUtil.loadAsset<GameObject> ("core/prefabs/ui", "TempButton"); //TODO replace with final button version
		GameObject inst = Instantiate(butPref, optionList, false);
		inst.transform.GetChild (0).GetComponent<Text> ().text = option.text;
		if (option.function == null)
			option.function = destroySelf;
		inst.GetComponent<Button> ().onClick.AddListener (option.function);
	}

	private void destroySelf()
	{
		Destroy (gameObject);
	}

	/* Misc */

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
}
