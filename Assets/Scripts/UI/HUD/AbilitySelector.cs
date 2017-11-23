using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AbilitySelector : MonoBehaviour
{
	/* Instance Vars */
	[SerializeField]
	private Image abilIcon;
	[SerializeField]
	private Toggle toggle;
	[SerializeField]
	private string abilityName;
	private Ability ability;

	/* Static Methods */


	/* Instance Methods */
	public void Awake()
	{
		toggle.onValueChanged.AddListener (toggleChanged);
	}

	#if UNITY_EDITOR
	public void Update()
	{
		ability = Ability.get (abilityName);
		if (ability != null)
		{
			abilIcon.sprite = ability.icon;
		}
	}
	#endif

	private void toggleChanged(bool v)
	{
		Entity player = GameManager.instance.player.GetComponent<Entity> ();
		if (v)
		{
			
			
		}
		else
		{

		}
	}
}
