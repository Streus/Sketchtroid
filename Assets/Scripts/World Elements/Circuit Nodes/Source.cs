using UnityEngine;
using System.Collections;

namespace CircuitNodes
{
	public class Source : CircuitNode
	{
		public override bool isActivated ()
		{
			return true;
		}

		public override void setActive (bool state)
		{
			Debug.LogWarning ("Cannot set " + gameObject.name + "; it is a Source.");
		}
	}
}
