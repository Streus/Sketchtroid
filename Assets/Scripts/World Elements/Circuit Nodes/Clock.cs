using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

namespace CircuitNodes
{
	public class Clock : CircuitNode, IReapable
	{
		[Tooltip("Sets the active state of the clock timer.")]
		[SerializeField]
		private bool running = false;
		[SerializeField]
		private Timer activeTimer = new Timer(1f);
		[SerializeField]
		private Timer inactiveTimer = new Timer(1f);
		[SerializeField]
		private bool startActive = true;

		public void Awake()
		{
			//complete the inactive timer
			if (startActive)
				inactiveTimer.Tick (inactiveTimer.Max);
			//complete the active timer
			else
				activeTimer.Tick (activeTimer.Max);
		}

		public void Update()
		{
			if (!running)
				return;

			if (activeTimer.Check ())
			{
				if (inactiveTimer.Tick (Time.deltaTime))
					activeTimer.Reset ();
			}
			else if (inactiveTimer.Check ())
			{
				if (activeTimer.Tick (Time.deltaTime))
					inactiveTimer.Reset ();
			}
		}

		public override bool IsActivated ()
		{
			return inactiveTimer.Check ();
		}

		public override void SetActive (bool state)
		{
			running = state;
		}
		
		#region IReapable implementation
		public SeedCollection.Base Reap ()
		{
			return new Seed (this);
		}

		public void Sow (SeedCollection.Base seed)
		{
			Seed s = (Seed)seed;
			running = s.running;
			activeTimer = s.activeTimer;
			inactiveTimer = s.inactiveTimer;
		}

		private class Seed : SeedCollection.Base
		{
			public bool running;
			public Timer activeTimer, inactiveTimer;

			public Seed(Clock subject)
			{
				running = subject.running;
				activeTimer = subject.activeTimer;
				inactiveTimer = subject.inactiveTimer;
			}
			public Seed(SerializationInfo info, StreamingContext context)
			{
				running = info.GetBoolean("running");
				activeTimer = (Timer)info.GetValue("a_timer", typeof(Timer));
				inactiveTimer = (Timer)info.GetValue("i_timer", typeof(Timer));
			}

			public override void GetObjectData (SerializationInfo info, StreamingContext context)
			{
				info.AddValue ("running", running);
				info.AddValue ("a_timer", activeTimer);
				info.AddValue ("i_timer", inactiveTimer);
			}
		}
		#endregion
	}
}
