using UnityEngine;

/// <summary>
/// A base class for classes used to modify the behavior of Bullets during runtime
/// </summary>
public abstract class BulletBehavior : ScriptableObject
{
	public abstract void OnStart (Bullet bullet);
	public abstract void OnUpdate(Bullet bullet);
	public abstract void OnFixedUpdate(Bullet bullet);
	public abstract void OnLateUpdate(Bullet bullet);
	public abstract void OnHit (Bullet bullet, Collider2D col, Entity hit);
	public abstract void OnDeath (Bullet bullet);
}
