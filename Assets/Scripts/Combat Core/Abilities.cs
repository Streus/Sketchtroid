using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class Ability
{
	static Ability()
	{
		latestID = 0;

		repository = new Dictionary<string, Ability> ();

		//player offensive
		repository.Add ("Spray", new Ability (
			"Spray",
			"Shoot a continuous stream of bullets",
			"UI_Ability_Spray",
			0.1f,
			"sprayShoot").assignID()
		);
		repository.Add ("Refract", new Ability (
			"Refract",
			"Fire a wide spread of 3 lasers",
			"UI_Ability_Refract",
			1f,
			"refractShoot").assignID()
		);
		repository.Add ("Lay Waste", new Ability (
			"Lay Waste",
			"Fire a large, slow projectile that splits into eight smaller projectiles on impact",
			"UI_Ability_LayWaste",
			2f,
			"lwShoot").assignID()
		);
		repository.Add ("Ricochet", new Ability (
			"Ricochet",
			"Shoot a piercing bullet that bounces three times before expiring",
			"UI_Ability_Ricochet",
			0.7f,
			"ricShoot").assignID()
		);

		//player mobility
		repository.Add ("Overdrive", new Ability (
			"Overdrive",
			"Increase maximum movespeed for a short time",
			"UI_Ability_Overdrive",
			5f,
			"overMove").assignID()
		);
		repository.Add ("Propel", new Ability (
			"Propel",
			"Gain a large burst of speed in one direction",
			"UI_Ability_Propel",
			3f,
			"propMove").assignID()
		);
		repository.Add ("Shift", new Ability (
			"Shift",
			"Teleport to a nearby location over 2 seconds",
			"UI_Ability_Shift",
			7f,
			"shiftMove").assignID()
		);
		repository.Add ("Phase", new Ability (
			"Phase",
			"Lose some movespeed, but gain the ability to pass through phase walls for a short time",
			"UI_Ability_Phase",
			10f,
			"phaseMove").assignID()
		);

		//player utility
		repository.Add ("Displace", new Ability (
			"Displace",
			"Fire off a wave that pushes objects away",
			"UI_Ability_Displace",
			2f,
			"dispUtil").assignID()
		);
		repository.Add ("Grapple", new Ability (
			"Grapple",
			"Thow out a grapple that attaches to any solid object",
			"UI_Ability_Grapple",
			4f,
			"grappUtil").assignID()
		);
		repository.Add ("Flash", new Ability (
			"Flash",
			"Discharge shield energy to stun enemies",
			"UI_Ability_Flash",
			6f,
			"flashUtil").assignID()
		);
		repository.Add ("Reflect", new Ability (
			"Reflect",
			"Empower the shield to reflect projectiles for a short time",
			"UI_Ability_Reflect",
			10f,
			"refUtil").assignID()
		);
	}

	// --- Player Offensive Abilities ---

	// Spray
	private bool sprayShoot(Entity subject, Vector2 targetPosition, params object[] args)
	{
		try
		{
			Bullet b = Bullet.create ("Basic", subject, (DamageType)args [0], subject.getFaction ());
			b.transform.position += subject.transform.up + (Vector3)UnityEngine.Random.insideUnitCircle * 0.3f;
			return true;
		}
		#pragma warning disable 0168
		catch(InvalidCastException ice)
		#pragma warning restore 0168
		{ 
			Debug.LogError ("Passed invalid argument to sprayShoot");
		}
		return false;
	}

	// Refract
	private bool refractShoot(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return true;
	}

	// Lay Waste
	private bool lwShoot(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return true;
	}

	// Ricochet
	private bool ricShoot(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return true;
	}

	// --- Player Mobility Abilities ---

	// Overdrive
	private bool overMove(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return true;
	}

	// Propel
	private bool propMove(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return true;
	}

	// Shift
	private bool shiftMove(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return true;
	}

	// Phase
	private bool phaseMove(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return true;
	}

	// --- Player Utility Abilities ---

	// Displace
	private bool dispUtil(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return true;
	}

	// Grapple
	private bool grappUtil(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return true;
	}

	// Flash
	private bool flashUtil(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return true;
	}

	// Reflect
	private bool refUtil(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return true;
	}
}
