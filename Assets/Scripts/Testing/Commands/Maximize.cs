using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Maximize : Command
	{
		public override string getHelp ()
		{
			return "Make the console fill the screen. Reverse with minimize. Usage: " +
				"maximize";
		}

		public override string getInvocation ()
		{
			return "maximize";
		}

		public override string execute (params string[] args)
		{
			Console.log.maximize ();
			return "Maximized the console window.";
		}
	}
}
