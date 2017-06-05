using UnityEngine;
using System.Collections;

public interface IDestructable
{
	float health{ get; private set; }

	void damage(float amount);
}

