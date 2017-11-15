using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

namespace StatusComponents
{
	[System.Serializable]
	public class HealOverTime : StatusComponent
	{
		private float healPerTick;
		private float tickRate;
		private float currentTick;

		private float finalHeal;

		public HealOverTime(float healPerTick, float tickRate) : base(1)
		{
			this.healPerTick = healPerTick;
			this.tickRate = tickRate;
			currentTick = tickRate;

			finalHeal = healPerTick;
		}
		public HealOverTime(HealOverTime other) : base(other)
		{
			this.healPerTick = other.healPerTick;
			this.tickRate = other.tickRate;
			currentTick = tickRate;

			finalHeal = healPerTick;
		}
		public HealOverTime(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			healPerTick = info.GetSingle ("healPerTick");
			tickRate = info.GetSingle ("tickRate");
			currentTick = info.GetSingle ("currentTick");

			finalHeal = info.GetSingle ("finalHeal");
		}

		public override void OnUpdate (Entity subject, float time)
		{
			currentTick -= time;
			if (currentTick <= 0)
			{
				Entity.healEntity (subject, healPerTick);
				if (parent.duration >= tickRate)
					currentTick = tickRate;
				else
				{
					finalHeal = healPerTick * (parent.duration / tickRate);
				}
			}
		}

		public override void OnRevert (Entity subject)
		{
			Entity.healEntity (subject, finalHeal);
		}

		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);
			info.AddValue ("healPerTick", healPerTick);
			info.AddValue ("tickRate", tickRate);
			info.AddValue ("currentTick", currentTick);

			info.AddValue ("finalHeal", finalHeal);
		}
	}
}
