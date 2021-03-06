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

		public override int execute (params string[] args)
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
					SceneStateManager.GetInstance ().TransitionTo (jumpRoom);
					Console.println ("Transitioned to " + jumpRoom, Console.Tag.info);
				}
				else
				{
					SceneStateManager.GetInstance ().JumpTo (jumpRoom);
					Console.println ("Jumped to " + jumpRoom, Console.Tag.info);
				}
			}
			#pragma warning disable 0168
			catch(System.ArgumentException ae)
			#pragma warning restore 0168
			{
				Console.println ("The scene " + jumpRoom + " is invalid.", Console.Tag.error);
				return Console.EXEC_FAILURE;
			}

			Console.println ("Successfully jumped to " + jumpRoom + "!");
			return Console.EXEC_SUCCESS;
		}
	}
}
