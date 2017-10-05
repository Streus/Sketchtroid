using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
	public abstract class Fork : ScriptableObject
	{
		public abstract bool check (Controller c);
	}
}