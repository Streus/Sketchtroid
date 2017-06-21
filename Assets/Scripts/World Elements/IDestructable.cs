using UnityEngine;
using System.Collections;

public interface IDestructable
{
	float health{ get; }

	void damage(float amount);
}
