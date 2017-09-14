using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
	/* Static Vars */
	private static HUDManager _instance;
	public static HUDManager instance { get { return _instance; } }

	/* Instance Vars */

	// An element that overlays the entire screen. Is used for fade transitions.
	[SerializeField]
	private Image fadeMask;

	/* Instance Methods */
	public void Awake()
	{
		if (_instance == null)
			_instance = this;
		else
			Debug.LogError ("More than one HUDManager in the scene");
	}

	public Image getFadeMask()
	{
		return fadeMask;
	}
}
