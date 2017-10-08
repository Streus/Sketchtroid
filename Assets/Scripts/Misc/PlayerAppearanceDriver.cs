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
		
	}

	private void partRemoved(Ability a)
	{

	}

	private void partSwapped(Ability a, Ability b, int index)
	{

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

	private void setCone(int skinIndex)
	{
		
	}
}
