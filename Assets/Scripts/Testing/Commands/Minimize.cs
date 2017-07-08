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
			return "minimize";
		}

		public override string execute (params string[] args)
		{
			Console.log.minimize ();
			return "Minimized the console window.";
		}
	}
}
