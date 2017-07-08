using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Help : Command
	{
		public override string getHelp ()
		{
			return "Displays the help text for all available commands, or a single " +
				"command. Usage: help [command]";
		}

		public override string getInvocation ()
		{
			return "help";
		}

		public override string execute (params string[] args)
		{
			Command[] commands = Console.log.getCommandList ();
			if (args.Length == 1)
			{
				foreach (Command c in commands)
					Console.log.println (c.getInvocation() + " | " + c.getHelp (), Console.LogTag.command_out);
			}
			else
			{
				foreach (Command c in commands)
				{
					if (c.getInvocation () == args [1]) 
					{
						Console.log.println (c.getInvocation() + " | " + c.getHelp (), Console.LogTag.command_out);
						return "";
					}
				}
				throw new ExecutionException ("Unknown command: " + args [1]);
			}
			return "";
		}
	}
}
