using UnityEngine;
using System.Collections;

namespace CircuitNodes
{
	public class AndGate : CircuitNode
	{
		[SerializeField]
		private CircuitNode[] targets;

		[Tooltip("With this toggled on, this will act as a NAND gate")]
		[SerializeField]
		private bool invert;

		public override bool isActivated ()
		{
			if (targets == null)
				return false;

			bool activated = true;
			for (int i = 0; i < targets.Length; i++)
			{
				if (targets[i] == null || !targets [i].isActivated ())
				{
					activated = false;
					break;
				}
			}

			return invert ? !activated : activated;
		}

		public override void setActive (bool state)
		{
			Debug.LogWarning ("Cannot set " + gameObject.name + "; it is a (N)AND gate.");
		}

		public override void OnDrawGizmos ()
		{
			base.OnDrawGizmos ();
			Gizmos.color = Color.gray;
			if (targets == null)
				return;
			for (int i = 0; i < targets.Length; i++)
			{
				if (targets [i] != null)
				{
					Vector3 dir = targets [i].transform.position - transform.position;
					Gizmos.DrawLine (transform.position + dir.normalized, targets [i].transform.position);
				}
			}
		}
	}
}
