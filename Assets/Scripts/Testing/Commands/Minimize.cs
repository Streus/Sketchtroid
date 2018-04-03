using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Minimize : Command
	{
		public override string getHelp ()
		{
			return "Tuck the console to the top the screen. Reverse with maximize. Usage: " +
				"minimize";
		}

		public override string getInvocation ()
		{
			return "min";
		}

		public override int execute (params string[] args)
		{
			Console.minimize ();
			return Console.EXEC_SUCCESS;
		}
	}
}
