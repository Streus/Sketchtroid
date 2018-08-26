using UnityEngine;
using System.Collections;

namespace CircuitNodes
{
	public class OrGate : CircuitNode
	{
		[SerializeField]
		private CircuitNode[] targets;

		[Tooltip("With this toggled on, this will act as a NOR gate")]
		[SerializeField]
		private bool invert;

		public override bool IsActivated ()
		{
			if (targets == null)
				return false;

			bool activated = false;
			for (int i = 0; i < targets.Length; i++)
			{
				if (targets[i] != null && targets [i].IsActivated ())
				{
					activated = true;
					break;
				}
			}

			return invert ? !activated : activated;
		}

		public override void SetActive (bool state)
		{
			Debug.LogWarning ("Cannot set " + gameObject.name + "; it is a (N)OR gate.");
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
