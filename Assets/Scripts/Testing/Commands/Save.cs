using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Save : Command
	{
		public override string getHelp ()
		{
			return "Save the current game. Usage: save [gameName]";
		}

		public override string getInvocation ()
		{
			return "save";
		}

		public override string execute (params string[] args)
		{
			int curIndex = 1;
			if (args.Length > 1)
				GameManager.instance.setGameName (args [curIndex++]);
			GameManager.instance.saveGame ();
			
			return "Saved the game.";
		}
	}
}
