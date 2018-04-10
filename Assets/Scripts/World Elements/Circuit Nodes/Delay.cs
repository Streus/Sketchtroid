using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace CircuitNodes
{
	public class Delay : Toggle
	{
		[SerializeField]
		private float delayTime = 1f;

		public override void Start ()
		{
			base.setActive (active);
		}

		private IEnumerator delayActive(bool state)
		{
			yield return new WaitForSeconds (delayTime);
			base.setActive (state);
		}

		public override void setActive (bool state)
		{
			StartCoroutine (delayActive (state));
		}

		public override SeedCollection.Base reap ()
		{
			return new DelaySeed (this);
		}

		public override void sow (SeedCollection.Base seed)
		{
			base.sow (seed);
			DelaySeed ds = (DelaySeed)seed;
			delayTime = ds.delayTime;
		}

		private class DelaySeed : Seed
		{
			public float delayTime;

			public DelaySeed(Delay d) : base(d)
			{
				delayTime = d.delayTime;
			}
			public DelaySeed(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				delayTime = info.GetSingle("delayTime");
			}

			public override void GetObjectData (SerializationInfo info, StreamingContext context)
			{
				base.GetObjectData (info, context);
				info.AddValue ("delayTime", delayTime);
			}
		}
	}
}
