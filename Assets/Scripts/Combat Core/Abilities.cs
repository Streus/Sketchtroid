﻿using System.Collections;
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
		Put(new Ability (
			"Spray",
			"Shoot a continuous stream of bullets",
			"UI_Ability_Spray",
			0.1f,
			0,
			"sprayShoot")
		);
		Put(new Ability (
			"Refract",
			"Fire a wide spread of 3 lasers",
			"UI_Ability_Refract",
			1f,
			0,
			"refractShoot")
		);
		Put(new Ability (
			"Lay Waste",
			"Fire a large, slow projectile that splits into eight smaller projectiles on impact",
			"UI_Ability_LayWaste",
			2f,
			0,
			"lwShoot")
		);
		Put(new Ability (
			"Ricochet",
			"Shoot a piercing bullet that bounces three times before expiring",
			"UI_Ability_Ricochet",
			0.7f,
			0,
			"ricShoot")
		);

		//player mobility
		Put(new Ability (
			"Overdrive",
			"Increase maximum movespeed for a short time",
			"UI_Ability_Overdrive",
			5f,
			0,
			"overMove")
		);
		Put(new Ability (
			"Propel",
			"Gain a large burst of speed in one direction",
			"UI_Ability_Propel",
			5f,
			3,
			"propMove")
		);
		Put(new Ability (
			"Shift",
			"Teleport to a nearby location over 2 seconds",
			"UI_Ability_Shift",
			7f,
			0,
			"shiftMove")
		);
		Put(new Ability (
			"Phase",
			"Lose some movespeed, but gain the ability to pass through phase walls for a short time",
			"UI_Ability_Phase",
			10f,
			0,
			"phaseMove")
		);

		//player utility
		Put(new Ability (
			"Displace",
			"Fire off a wave that pushes objects away",
			"UI_Ability_Displace",
			2f,
			0,
			"dispUtil")
		);
		Put(new Ability (
			"Grapple",
			"Thow out a grapple that attaches to any solid object",
			"UI_Ability_Grapple",
			4f,
			0,
			"grappUtil")
		);
		Put(new Ability (
			"Flash",
			"Discharge shield energy to stun enemies",
			"UI_Ability_Flash",
			6f,
			0,
			"flashUtil")
		);
		Put(new Ability (
			"Reflect",
			"Empower the shield to reflect projectiles for a short time",
			"UI_Ability_Reflect",
			10f,
			0,
			"refUtil")
		);
	}

	// --- Player Offensive Abilities ---

	// Spray
	private bool sprayShoot(Entity subject, Vector2 targetPosition, params object[] args)
	{
		Bullet b = Bullet.create ("Basic", subject, subject.DefaultDT, subject.GetFaction ());
		b.transform.position += subject.transform.up + (Vector3)UnityEngine.Random.insideUnitCircle * 0.3f;
		return true;
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
