using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ErrorDisplay : MonoBehaviour
{
	public Text textDisplay;

	private float displayTime;

	public void Awake ()
	{
		RectTransform rect = GetComponent<RectTransform>();
		rect.offsetMax = rect.offsetMin = Vector2.zero;

		gameObject.SetActive (false);
	}

	public void Update()
	{
		displayTime -= Time.deltaTime;
		if (displayTime <= 0f)
			dismissWindow ();
	}

	public void displayError(string errText, float displayTime = 3f)
	{
		if(textDisplay != null)
			textDisplay.text = errText;

		this.displayTime = displayTime;
	}

	public void dismissWindow()
	{
		gameObject.SetActive (false);
	}
}
