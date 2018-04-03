using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class TestParse : Command
	{
		public override string getHelp ()
		{
			return "Used to demonstrate the parse strategy used by the console. Usage: " +
				"testparse [examplecommand]";
		}

		public override string getInvocation ()
		{
			return "testparse";
		}

		public override int execute (params string[] args)
		{
			string str = "";
			str = "|" + args [0] + "|";
			for (int i = 1; i < args.Length; i++)
				str += " |" + args [i] + "|";
			Console.println(str);
			return Console.EXEC_SUCCESS;
		}
	}
}
