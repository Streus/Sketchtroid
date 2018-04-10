using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class SystemInformation : Command
	{
		public override string getHelp ()
		{
			return "Shows information about the current execution" +
				"environment. Usage: sysinfo";
		}

		public override string getInvocation ()
		{
			return "sysinfo";
		}

		public override int execute (params string[] args)
		{
			string str = "<b>[System Info]</b>\n";
			#if UNITY_EDITOR
			str += "<i>Is Editor Build </i>\n";
			#endif
			str += "<b>OS: </b>" + SystemInfo.operatingSystem + "\n";
			str += "<b>GPU: </b>" + SystemInfo.graphicsDeviceName + "\n";

			Console.print (str);
			return Console.EXEC_SUCCESS;
		}
	}
}
