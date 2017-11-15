﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class Jump : Command
	{
		public override string getHelp ()
		{
			return "Send the game to the indicated room. Usage: jump <room name> [-s]";
		}

		public override string getInvocation ()
		{
			return "jump";
		}

		public override string execute (params string[] args)
		{
			bool saveData = false;
			string jumpRoom = "";

			jumpRoom = args [1];
			if (args.Length > 2)
				saveData = args [2] == "-s";

			try
			{
				if (saveData)
				{
					SceneStateManager.instance ().transitionTo (jumpRoom);
					Console.log.println ("Transitioned to " + jumpRoom, Console.LogTag.info);
				}
				else
				{
					SceneStateManager.instance ().jumpTo (jumpRoom);
					Console.log.println ("Jumped to " + jumpRoom, Console.LogTag.info);
				}
			}
			catch(System.ArgumentException ae)
			{
				Console.log.println ("The scene " + jumpRoom + " is invalid.", Console.LogTag.error);
				return "";
			}

			return "Successfully changed scene!";
		}
	}
}