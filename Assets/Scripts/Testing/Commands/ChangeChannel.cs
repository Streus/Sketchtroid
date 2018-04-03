using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class ChangeChannel : Command
	{
		public override string getHelp ()
		{
			return "Changes the current console channel. Usage: ch <(channel number) | (channel name)>";
		}

		public override string getInvocation ()
		{
			return "ch";
		}

		public override int execute (params string[] args)
		{
			int channelNum;
			if (int.TryParse (args [1], out channelNum))
				Console.setChannel (channelNum);
			else
			{
				try
				{
					Console.setChannel(Console.nameToChannel(args[1]));
				}
				catch(System.InvalidOperationException ioe)
				{
					throw new ExecutionException (ioe.Message);
				}
			}
			return Console.EXEC_SUCCESS;
		}
	}
}
