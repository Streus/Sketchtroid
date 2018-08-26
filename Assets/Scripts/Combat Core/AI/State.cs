using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
	[CreateAssetMenu(menuName = "AI/State")]
	public class State : ScriptableObject
	{
		public Color color = Color.grey;
		public Action enterAction;
		public Action[] actions;
		public Action exitAction;
		public Path[] paths;

		// Perform an action when this state is entered
		public void enter(Controller c)
		{
			if(enterAction != null)
				enterAction.perform (c);
		}

		// Perform all actions in the set of actions,
		// then check for a fork condition
		public void update (Controller c)
		{
			foreach (Action a in actions)
				a.perform (c);

			foreach (Path p in paths)
			{
				if (p.decision.check (c))
				{
					if (p.success != null)
						c.SetState (p.success);
				}
				else if (p.failure != null)
					c.SetState (p.failure);
			}
		}

		// Perform an action when this state is exited
		public void exit(Controller c)
		{
			if(exitAction != null)
				exitAction.perform (c);
		}

		[System.Serializable]
		public class Path
		{
			public State success, failure;
			public Fork decision;
		}
	}
}
