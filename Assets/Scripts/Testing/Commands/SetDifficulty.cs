using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class SetDifficulty : Command
	{
		public override string getHelp ()
		{
			return "Sets the difficulty of the current game. Usage: " +
				"difficulty <[easy|0]|[normal|1]|[hard|2]|[expert|3]|[master|4]>";
		}

		public override string getInvocation ()
		{
			return "difficulty";
		}

		public override int execute (params string[] args)
		{
			Difficulty newDiff = GameManager.instance.difficulty;
			args [1] = args [1].ToLower ();
			if (args [1] == "easy" || args [1] == "0")
				newDiff = Difficulty.easy;
			else if (args [1] == "normal" || args [1] == "1")
				newDiff = Difficulty.normal;
			else if (args [1] == "hard" || args [1] == "2")
				newDiff = Difficulty.hard;
			else if (args [1] == "expert" || args [1] == "3")
				newDiff = Difficulty.expert;
			else if (args [1] == "master" || args [1] == "4")
				newDiff = Difficulty.master;
			else
				throw new ExecutionException ("No such difficulty: " + args [1]);

			GameManager.instance.difficulty = newDiff;
			
			Console.println ("Set difficulty to " + GameManager.instance.difficulty.ToString ());
			return Console.EXEC_SUCCESS;
		}
	}
}
