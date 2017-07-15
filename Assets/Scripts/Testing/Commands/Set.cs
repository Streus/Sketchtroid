using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Set : Command
	{
		public override string getHelp ()
		{
			return "Declares and instantiates a variable. Variables only hold strings. " +
				"-g makes the variable global. Usage: set [-g] <name> [=] <value>";
		}

		public override string getInvocation ()
		{
			return "set";
		}

		public override string execute (params string[] args)
		{
			int currArg = 1;
			bool globalVar = false;
			string name, value;

			if (args [currArg] == "-g")
			{
				globalVar = true;
				currArg++;
			}

			name = args [currArg];
			currArg++;
			if (args [currArg] == "=")
				currArg++;
			value = args [currArg];

			if (Console.log.declareVariable (name, value, globalVar))
				return "Created new variable \"" + name + "\" with value \"" + value + "\"";
			else if(Console.log.setVariableValue(name, value, globalVar))
				return "Set existing variable \"" + name + "\" to value \"" + value + "\"";
			return "";
		}
	}
}
