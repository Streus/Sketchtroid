using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAppearanceDriver : MonoBehaviour
{
	[SerializeField]
	private Entity entity;

	[SerializeField]
	private string partsPath = "Prefabs/Entities/Player Parts/";

	[SerializeField]
	private GameObject cone;
	[SerializeField]
	private GameObject leftWing;
	[SerializeField]
	private GameObject rightWing;
	[SerializeField]
	private GameObject engine;

	[SerializeField]
	private SpriteRenderer dtSymbol;
	[SerializeField]
	private Sprite[] symbols;

	public void Awake()
	{
		entity.abilityAdded += partAdded;
		entity.abilityRemoved += partRemoved;
		entity.abilitySwapped += partSwapped;
		entity.damageTypeChanged += dtChanged;
	}

	public void Start()
	{
		
	}

	private void init()
	{
		for (int i = 0; i < entity.abilityCount; i++)
		{

		}
	}

	private void partAdded(Ability a)
	{
		partSwapped (a, a, 0);
	}

	private void partRemoved(Ability a)
	{
		GameObject[] existing = abilityToPart (a.name);
		foreach (GameObject g in existing)
		{
			Destroy (g);
		}
	}

	private void partSwapped(Ability a, Ability b, int index)
	{
		GameObject pref = Resources.Load<GameObject> (partsPath + a.name);
		GameObject[] existing = abilityToPart (b.name);
		for (int i = 0; i < existing.Length; i++)
		{
			Destroy (existing [i]);
			existing [i] = Instantiate (pref, entity.transform, false);
			if (i == 1)
				existing [i].transform.localScale = new Vector3 (
					existing [i].transform.localScale.x,
					existing [i].transform.localScale.y,
					-existing [i].transform.localScale.z);
		}
	}

	private void dtChanged(DamageType dt)
	{
		if (dt == DamageType.NONE)
		{
			dtSymbol.sprite = null;
			return;
		}

		dtSymbol.sprite = symbols [((int)dt) - 1];
		dtSymbol.color = Bullet.damageTypeToColor (dt);
	}

	private GameObject[] abilityToPart(string s)
	{
		if (s == "Spray" || s == "Refract" || s == "Lay Waste" || s == "Ricochet")
		{
			return new GameObject[]{ cone };
		}
		else if (s == "Overdrive" || s == "Propel" || s == "Shift" || s == "Phase")
		{
			return new GameObject[]{ leftWing, rightWing };
		}
		else if (s == "Displace" || s == "Grapple" || s == "Flash" || s == "Reflect")
		{
			return new GameObject[]{ engine };
		}
		else
			return new GameObject[]{ };
	}
}
