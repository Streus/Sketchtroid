using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace CircuitNodes
{
	public class Toggle : CircuitNode, IReapable
	{
		[SerializeField]
		private bool active = false;

		[Tooltip("Will attempt to lock these to this toggle's internal state")]
		[SerializeField]
		private CircuitNode[] targets;

		public void Awake()
		{
			setActive (active);
		}

		public override bool isActivated ()
		{
			return active;
		}

		public override void setActive (bool state)
		{
			active = state;
			if (targets != null)
			{
				for (int i = 0; i < targets.Length; i++)
				{
					if (targets [i] != null)
						targets [i].setActive (active);
				}
			}
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

		#region IReapable implementation

		public virtual SeedCollection.Base reap ()
		{
			return new Seed (this);
		}

		public virtual void sow (SeedCollection.Base seed)
		{
			Seed s = (Seed)seed;
			active = s.active;
		}

		protected class Seed : SeedCollection.Base
		{
			public bool active;

			public Seed(Toggle t)
			{
				active = t.active;
			}
			public Seed(SerializationInfo info, StreamingContext context)
			{
				active = info.GetBoolean("active");
			}

			public override void GetObjectData (SerializationInfo info, StreamingContext context)
			{
				info.AddValue ("active", active);
			}
		}
		#endregion
	}
}
