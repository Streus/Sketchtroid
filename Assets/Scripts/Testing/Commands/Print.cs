using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Print : Command
	{
		public override string getHelp ()
		{
			return "Print to the console. -e for error, -w for warning" +
				", -i for normal output. Usage: print [-e|-w|-i] [\"message\"]";
		}

		public override string getInvocation ()
		{
			return "print";
		}

		public override int execute (params string[] args)
		{
			Console.Tag tag = Console.Tag.none;
			int mesgArg = 2;
			if (args [1] == "-e")
				tag = Console.Tag.error;
			else if (args [1] == "-w")
				tag = Console.Tag.warning;
			else if (args [1] == "-i")
				tag = Console.Tag.info;
			else
				mesgArg = 1;

			try
			{
				Console.println (args [mesgArg], tag);
			}
			#pragma warning disable 0168
			catch(System.IndexOutOfRangeException ioore)
			#pragma warning restore 0168
			{
				Console.println ("", tag);
				return Console.EXEC_FAILURE;
			}

			return Console.EXEC_SUCCESS;
		}
	}
}
