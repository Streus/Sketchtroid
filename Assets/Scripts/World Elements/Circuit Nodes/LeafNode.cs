using UnityEngine;
using System.Collections;

namespace CircuitNodes
{
	public class LeafNode : CircuitNode
	{
		[Tooltip("The node to watch")]
		[SerializeField]
		private CircuitNode target;

		[SerializeField]
		private bool invert;

		public override bool IsActivated ()
		{
			if (target != null)
				return invert ? !target.IsActivated () : target.IsActivated();
			return false;
		}

		public override void SetActive (bool state)
		{
			Debug.LogWarning ("Cannot set " + gameObject.name + "; it is a Leaf node.");
		}

		public override void OnDrawGizmos ()
		{
			base.OnDrawGizmos ();
			Gizmos.color = Color.gray;
			if (target != null)
			{
				Vector3 dir = target.transform.position - transform.position;
				Gizmos.DrawLine (transform.position + dir.normalized, target.transform.position);
			}
		}
	}
}
