using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAppearanceDriver : MonoBehaviour
{
	[SerializeField]
	private Entity entity;

	[Header("Parts")]
	private GameObject cone;
	private GameObject leftWing;
	private GameObject rightWing;
	private GameObject engine;

	[SerializeField]
	private Part defaultCone;
	[SerializeField]
	private Part defaultWings;
	[SerializeField]
	private Part defaultEngine;

	[SerializeField]
	private List<Part> parts;

	[Header("Symbol")]
	[SerializeField]
	private SpriteRenderer dtSymbol;
	[SerializeField]
	private Sprite[] symbols;

	public void Awake()
	{
		if (entity != null)
		{
			entity.abilityAdded += partAdded;
			entity.abilityRemoved += partRemoved;
			entity.abilitySwapped += partSwapped;
			entity.damageTypeChanged += dtChanged;
		}

		addPart (defaultCone);
		addPart (defaultWings);
		addPart (defaultEngine);
	}

	// Initialize using an Entity's Seed data
	public void init(Entity.Seed data)
	{
		for (int i = 0; i < data.abilities.Length; i++)
		{
			partAdded (data.abilities [i], i);
		}
	}

	private void partAdded(Ability a, int index = -1)
	{
		Part newPart = abilityToPart (a);
		if (newPart != null)
			addPart (newPart);
	}

	private void partRemoved(Ability a, int index = -1)
	{
		Part oldPart = abilityToPart (a);
		if (oldPart != null)
			removePart (oldPart);
	}

	private void partSwapped(Ability a, Ability b, int index)
	{
		partRemoved (b);
		if (a != null)
			partAdded (a);
		else
		{
			switch (abilityToPart (b).section)
			{
			case Section.cone:
				addPart (defaultCone);
				break;
			case Section.wings:
				addPart (defaultWings);
				break;
			case Section.engine:
				addPart (defaultEngine);
				break;
			}
		}
	}

	private Part abilityToPart(Ability a)
	{
		if (a == null)
			return null;

		return parts.Find (delegate(Part obj) {
			return a.name == obj.abilityName;
		});
	}

	private void removePart(Part p)
	{
		switch (p.section)
		{
		case Section.cone:
			addPart (defaultCone);
			break;
		case Section.wings:
			addPart (defaultWings);
			break;
		case Section.engine:
			addPart (defaultEngine);
			break;
		}
		/*
		switch (p.section)
		{
		case Section.cone:
			Destroy (cone);
			cone = Instantiate<GameObject> (defaultCone.prefab, transform, false);
			break;
		case Section.wings:
			Destroy (leftWing);
			Destroy (rightWing);
			leftWing = Instantiate<GameObject> (defaultWings.prefab, transform, false);
			rightWing = Instantiate<GameObject> (defaultWings.prefab, transform, false);
			rightWing.transform.localScale = new Vector3 (
				-rightWing.transform.localScale.x,
				rightWing.transform.localScale.y,
				rightWing.transform.localScale.z);
			break;
		case Section.engine:
			Destroy (engine);
			engine = Instantiate<GameObject> (defaultEngine.prefab, transform, false);
			break;
		}
		*/
	}

	private void addPart(Part p)
	{
		switch (p.section)
		{
		case Section.cone:
			Destroy (cone);
			cone = Instantiate<GameObject> (p.prefab, transform, false);
			break;
		case Section.wings:
			Destroy (leftWing);
			Destroy (rightWing);
			leftWing = Instantiate<GameObject> (p.prefab, transform, false);
			rightWing = Instantiate<GameObject> (p.prefab, transform, false);
			rightWing.transform.localScale = new Vector3 (
				-rightWing.transform.localScale.x,
				rightWing.transform.localScale.y,
				rightWing.transform.localScale.z);
			break;
		case Section.engine:
			Destroy (engine);
			engine = Instantiate<GameObject> (p.prefab, transform, false);
			break;
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
		dtSymbol.color = Bullet.DamageTypeToColor (dt);
	}

	[System.Serializable]
	public class Part
	{
		public string abilityName;
		public GameObject prefab;
		public Section section;

		public Part(string a, GameObject p, Section s)
		{
			abilityName = a;
			prefab = p;
			section = s;
		}
	}

	public enum Section
	{
		cone, engine, wings
	}
}
