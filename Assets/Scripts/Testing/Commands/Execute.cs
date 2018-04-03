using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Execute : Command
	{
		public override string getHelp ()
		{
			return "Execute a CSER file. Usage: exec <filepath>";
		}

		public override string getInvocation ()
		{
			return "exec";
		}

		public override int execute (params string[] args)
		{
			return Console.log.executeFile (args [1]) ? Console.EXEC_SUCCESS : Console.EXEC_FAILURE;
		}
	}
}
