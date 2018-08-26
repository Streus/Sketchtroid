using UnityEngine;
using System.Collections;

namespace CircuitNodes
{
	public class XorGate : CircuitNode
	{
		[SerializeField]
		private CircuitNode[] targets;

		[Tooltip("With this toggled on, this will act as a XNOR gate")]
		[SerializeField]
		private bool invert;

		public override bool IsActivated ()
		{
			if (targets == null)
				return false;

			bool activated = false;
			for (int i = 0; i < targets.Length; i++)
			{
				if (targets [i] != null && targets [i].IsActivated ())
				{
					if (!activated)
						activated = true;
					else
					{
						activated = false;
						break;
					}
				}
			}

			return invert ? !activated : activated;
		}

		public override void SetActive (bool state)
		{
			Debug.LogWarning ("Cannot set " + gameObject.name + "; it is a XOR gate.");
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
