using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class StartScope : Command
	{
		public override string getHelp ()
		{
			return "Starts a new scope for variables. Usage: do";
		}

		public override string getInvocation ()
		{
			return "do";
		}

		public override int execute (params string[] args)
		{
			Console.log.openScope ();
			return Console.EXEC_SUCCESS;
		}
	}
}
