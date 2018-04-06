using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class SystemInformation : Command
	{
		public override string getHelp ()
		{
			return "Shows information about the current execution" +
				"environment. Usage: sysinfo";
		}

		public override string getInvocation ()
		{
			return "sysinfo";
		}

		public override int execute (params string[] args)
		{
			Console.print (Console.log.getSystemInfo ());
			return Console.EXEC_SUCCESS;
		}
	}
}
