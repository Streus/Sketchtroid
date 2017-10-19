﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class Ability
{
	static Ability()
	{
		repository = new Dictionary<string, Ability> ();

		//player offensive
		repository.Add ("Spray", new Ability (
			"Spray",
			"Shoot a continuous stream of bullets",
			"",
			0.1f,
			"sprayShoot")
		);
		repository.Add ("Refract", new Ability (
			"Refract",
			"Fire a wide spread of 3 lasers",
			"",
			1f,
			"refractShoot")
		);
		repository.Add ("Lay Waste", new Ability (
			"Lay Waste",
			"Fire a large, slow projectile that splits into eight smaller projectiles on impact",
			"",
			2f,
			"lwShoot")
		);
		repository.Add ("Ricochet", new Ability (
			"Ricochet",
			"Shoot a piercing bullet that bounces three times before expiring",
			"",
			0.7f,
			"ricShoot")
		);

		//player mobility
		repository.Add ("Overdrive", new Ability (
			"Overdrive",
			"Increase maximum movespeed for a short time",
			"",
			5f,
			"overMove")
		);
		repository.Add ("Propel", new Ability (
			"Propel",
			"Gain a large burst of speed in one direction",
			"",
			3f,
			"propMove")
		);
		repository.Add ("Shift", new Ability (
			"Shift",
			"Teleport to a nearby location over 2 seconds",
			"",
			7f,
			"shiftMove")
		);
		repository.Add ("Phase", new Ability (
			"Phase",
			"Lose some movespeed, but gain the ability to pass through phase walls for a short time",
			"",
			10f,
			"phaseMove")
		);

		//player utility
		repository.Add ("Displace", new Ability (
			"Displace",
			"Fire off a wave that pushes objects away",
			"",
			2f,
			"dispUtil")
		);
		repository.Add ("Grapple", new Ability (
			"Grapple",
			"Thow out a grapple that attaches to any solid object",
			"",
			4f,
			"grappUtil")
		);
		repository.Add ("Flash", new Ability (
			"Flash",
			"Discharge shield energy to stun enemies",
			"",
			6f,
			"flashUtil")
		);
		repository.Add ("Reflect", new Ability (
			"Reflect",
			"Empower the shield to reflect projectiles for a short time",
			"",
			10f,
			"refUtil")
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
		return false;
	}

	// Lay Waste
	private bool lwShoot(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return false;
	}

	// Ricochet
	private bool ricShoot(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return false;
	}

	// --- Player Mobility Abilities ---

	// Overdrive
	private bool overMove(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return false;
	}

	// Propel
	private bool propMove(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return false;
	}

	// Shift
	private bool shiftMove(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return false;
	}

	// Phase
	private bool phaseMove(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return false;
	}

	// --- Player Utility Abilities ---

	// Displace
	private bool dispUtil(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return false;
	}

	// Grapple
	private bool grappUtil(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return false;
	}

	// Flash
	private bool flashUtil(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return false;
	}

	// Reflect
	private bool refUtil(Entity subject, Vector2 targetPosition, params object[] args)
	{
		return false;
	}
}
