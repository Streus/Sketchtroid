﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Maximize : Command
	{
		public override string getHelp ()
		{
			return "Make the console fill the screen. Reverse with minimize. Usage: " +
				"maximize";
		}

		public override string getInvocation ()
		{
			return "max";
		}

		public override int execute (params string[] args)
		{
			Console.maximize ();
			return Console.EXEC_SUCCESS;
		}
	}
}
