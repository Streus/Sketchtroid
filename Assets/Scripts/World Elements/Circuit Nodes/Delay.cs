using UnityEngine;
using System.Collections;

namespace CircuitNodes
{
	public class Delay : CircuitNode
	{
		[Tooltip("The node's state to watch")]
		[SerializeField]
		private CircuitNode parent;

		[SerializeField]
		private Timer delay = new Timer(1f);

		// A stored state of the parent node
		private bool? prevState = null;

		public void Update()
		{
			//TODO Delay circuit node behavior
		}
			
		public override bool isActivated ()
		{
			return delay.check ();
		}

		public override void setActive (bool state)
		{
			Debug.LogWarning ("Cannot directly set " + gameObject.name + "; Delays watch a parent.");
		}

		//TODO Delay serialization
	}
}
