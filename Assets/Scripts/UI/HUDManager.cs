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

	// The Entity from which the HUD will pull values.
	[SerializeField] //DEBUG
	private Entity subject;

	// Various UI elements that correspond to data in the subject entity
	[SerializeField]
	private Image healthBar;
	[SerializeField]
	private Image shieldBar;
	//TODO add more fields to the HUDManager

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

	public void Update()
	{
		healthBar.fillAmount = subject.healthPerc;
		shieldBar.fillAmount = subject.shieldPerc;
	}

	public void setSubject(Entity subject)
	{
		this.subject = subject;

		//TODO other clearing and resetting of elements?
	}

	public Image getFadeMask()
	{
		return fadeMask;
	}
}
