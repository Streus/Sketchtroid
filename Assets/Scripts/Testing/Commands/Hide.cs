using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Hide : Command
	{
		public override string getHelp ()
		{
			return "Hides the console window.  Usage: hide";
		}

		public override string getInvocation ()
		{
			return "hide";
		}

		public override int execute (params string[] args)
		{
			Console.log.isEnabled = false;
			return Console.EXEC_SUCCESS;
		}
	}
}
