using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class ListChannels : Command
	{
		public override string getHelp ()
		{
			return "Lists all of the named channels. Usage: lcs";
		}

		public override string getInvocation ()
		{
			return "lcs";
		}

		public override int execute (params string[] args)
		{
			if (Console.log == null)
				return Console.EXEC_FAILURE;

			string[] names = Console.log.getChannelNames ();
			for (int i = 0; i < names.Length; i++)
			{
				if (names [i] != "")
				{
					Console.println (" (" + i + ") " + names [i], Console.Tag.info);
				}
			}

			return Console.EXEC_SUCCESS;
		}
	}
}
