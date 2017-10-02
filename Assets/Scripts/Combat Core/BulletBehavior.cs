using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base class for classes used to modify the behavior of Bullets during runtime
/// </summary>
public abstract class BulletBehavior : ScriptableObject
{
	public abstract void start (Bullet bullet);
	public abstract void update(Bullet bullet);
	public abstract void fixedUpdate(Bullet bullet);
	public abstract void lateUpdate(Bullet bullet);
	public abstract void onHit (Bullet bullet, Collider2D col, Entity hit);
	public abstract void onDeath (Bullet bullet);
}
