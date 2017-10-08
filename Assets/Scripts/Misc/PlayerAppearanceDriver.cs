using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAppearanceDriver : MonoBehaviour
{
	[SerializeField]
	private Entity entity;

	[SerializeField]
	private VisualComponent cone;
	[SerializeField]
	private VisualComponent dtSymbol;
	[SerializeField]
	private VisualComponent leftWing;
	[SerializeField]
	private VisualComponent rightWing;
	[SerializeField]
	private VisualComponent engine;

	public void Awake()
	{
		entity.abilityAdded += partAdded;
		entity.abilityRemoved += partRemoved;
		entity.abilitySwapped += partSwapped;
		entity.damageTypeChanged += dtChanged;

//		cone.part.enabled = dtSymbol.part.enabled = leftWing.part.enabled = 
//			rightWing.part.enabled = engine.part.enabled = false;
	}

	public void Start()
	{

	}

	void Player_damageTypeChanged (DamageType dt)
	{
		
	}

	private void init()
	{
		
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
			dtSymbol.part.sprite = null;
			return;
		}

		dtSymbol.part.sprite = dtSymbol.skins [((int)dt) - 1];
		dtSymbol.part.color = Bullet.damageTypeToColor (dt);
	}

	[System.Serializable]
	public struct VisualComponent
	{
		public SpriteRenderer part;
		public PolygonCollider2D col; // col.Reset() ???
		public Sprite[] skins;
	}
}
