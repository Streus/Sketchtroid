using UnityEngine;
using System.Runtime.Serialization;
using System;

[Serializable]
public class SeedBase: ISerializable
{
	/* Instance Vars */

	// The GameObject from which this seed will pull values
	private GameObject subject;

	// Values for filling a transform
	public Vector3 tPosition;
	public Quaternion tRotation;

	// Values for filling a Rigidbody2D
	public Rigidbody2D rb2d;

	/* Constructors */

	// Create a new Seed that will pull transform and rigidbody info when serialized
	public SeedBase(GameObject subject)
	{
		this.subject = subject;
		tPosition = default(Vector3);
		tRotation = default(Quaternion);
		rb2d = null;

	}

	// Create a Seed from serialized data that contains a transform and rigidbody state
	public SeedBase(SerializationInfo info, StreamingContext context)
	{
		tPosition = new Vector3 (
			info.GetSingle ("t.p.x"),
			info.GetSingle ("t.p.y"),
			info.GetSingle ("t.p.z"));
		tRotation = new Quaternion (
			info.GetSingle ("t.r.x"),
			info.GetSingle ("t.r.y"),
			info.GetSingle ("t.r.z"),
			info.GetSingle ("t.r.w"));

		rb2d = new Rigidbody2D ();
		try
		{
			rb2d.position = new Vector2(
				info.GetSingle("rb.p.x"),
				info.GetSingle("rb.p.y"));
			rb2d.rotation = info.GetSingle("rb.r");
			rb2d.velocity = new Vector2(
				info.GetSingle("rb.v.x"),
				info.GetSingle("rb.v.y"));
			rb2d.angularVelocity = info.GetSingle("rb.av");
		}
		catch(SerializationException se){ }
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		//save transform values
		Transform t = subject.transform;
		info.AddValue ("t.p.x", t.position.x);
		info.AddValue ("t.p.y", t.position.y);
		info.AddValue ("t.p.z", t.position.z);
		info.AddValue ("t.r.x", t.rotation.x);
		info.AddValue ("t.r.y", t.rotation.y);
		info.AddValue ("t.r.z", t.rotation.z);
		info.AddValue ("t.r.w", t.rotation.w);

		//save rigidbody information, if needed
		Rigidbody2D rb = subject.GetComponent<Rigidbody2D> ();
		if (rb != null)
		{
			info.AddValue ("rb.p.x", rb.position.x);
			info.AddValue ("rb.p.y", rb.position.y);
			info.AddValue ("rb.r", rb.rotation);
			info.AddValue ("rb.v.x", rb.velocity.x);
			info.AddValue ("rb.v.x", rb.velocity.y);
			info.AddValue ("rb.av", rb.angularVelocity);
		}
	}
}