using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

[Serializable]
public class Status : ISerializable
{
	/* Static Vars */


	/* Instance Vars */

	// The displayed name of this Status
	public readonly string name;

	// The base description of this Status
	public readonly string desc;

	// The path to the Sprite associated with this Status
	public readonly string iconPath;

	// The time this Status will exist until it expires
	public float duration { get; private set; }

	/* Static Methods */


	/* Constructors */


	/* Instance Methods */
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{

	}
}
