using UnityEngine;
using System.Collections;

namespace CircuitNodes
{
	public class Source : CircuitNode
	{
		public override bool IsActivated ()
		{
			return true;
		}

		public override void SetActive (bool state)
		{
			Debug.LogWarning ("Cannot set " + gameObject.name + "; it is a Source.");
		}
	}
}
