using UnityEngine;
using System.Collections;

namespace CircuitNodes
{
	public class NotGate : CircuitNode
	{
		[Tooltip("The node watch and invert")]
		[SerializeField]
		private CircuitNode target;

		public override bool isActivated ()
		{
			if (target != null)
				return !target.isActivated ();
			return false;
		}

		public override void setActive (bool state)
		{
			Debug.LogWarning ("Cannot set " + gameObject.name + "; it is a NOT gate.");
		}

		public override void OnDrawGizmos ()
		{
			base.OnDrawGizmos ();
			Gizmos.color = Color.gray;
			Vector3 dir = target.transform.position - transform.position;
			Gizmos.DrawLine (transform.position + dir.normalized, target.transform.position);
		}
	}
}
