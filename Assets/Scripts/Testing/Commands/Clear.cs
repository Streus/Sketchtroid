using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Clear : Command
	{
		public override string getHelp ()
		{
			return "Clears the console ouput. Usage: clear";
		}

		public override string getInvocation ()
		{
			return "clear";
		}

		public override string execute (params string[] args)
		{
			Console.log.clear ();
			return "";
		}
	}
}
