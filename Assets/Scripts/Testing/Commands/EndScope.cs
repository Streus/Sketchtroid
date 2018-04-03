using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class EndScope : Command
	{
		public override string getHelp ()
		{
			return "Ends the current scope. Usage: end";
		}

		public override string getInvocation ()
		{
			return "end";
		}

		public override int execute (params string[] args)
		{
			Console.log.closeScope ();
			return Console.EXEC_SUCCESS;
		}
	}
}
