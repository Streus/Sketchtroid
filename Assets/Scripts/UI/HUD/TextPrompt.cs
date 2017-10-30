using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPrompt : MonoBehaviour
{
	/* Static Variables */


	/* Instance Variables */
	[SerializeField]
	private Text header;
	[SerializeField]
	private Text subText;

	/* Static Methods */

	// Create a text prompt with a given text value and duration
	public static TextPrompt create(RectTransform parent, string title, string caption, float duration)
	{
		GameObject pref = Resources.Load<GameObject> ("Prefabs/UI/HUD/TextPrompt");
		GameObject inst = Instantiate<GameObject> (pref, parent, false);
		TextPrompt tp = inst.GetComponent<TextPrompt> ();

		tp.header.text = title;
		tp.subText.text = caption;

		Destroy (inst, Mathf.Abs(duration));

		return tp;
	}

	/* Instance Methods */


}
