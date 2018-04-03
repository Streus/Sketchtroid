using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class DumpSSM : Command
	{
		public override string getHelp ()
		{
			return "Dump the state of the SceneStateManager to the console. Usage: dumpssm";
		}

		public override string getInvocation ()
		{
			return "dumpssm";
		}

		public override int execute (params string[] args)
		{
			Console.print (SceneStateManager.getInstance ().ToString ());
			return Console.EXEC_SUCCESS;
		}
	}
}
