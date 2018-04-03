using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Commands
{
	public class Exit : Command
	{
		public override string getHelp ()
		{
			return "Exit the application immediately. Usage: exit";
		}

		public override string getInvocation ()
		{
			return "exit";
		}

		public override int execute (params string[] args)
		{
			Application.Quit ();
			#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
			#endif
			return Console.EXEC_SUCCESS;
		}
	}
}
