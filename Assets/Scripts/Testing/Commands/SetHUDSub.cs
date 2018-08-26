using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class SetHUDSub : Command
	{
		public override string getHelp ()
		{
			return "Sets the subject of the currently active HUD. Usage: " +
				"hudsub <entity name>";
		}

		public override string getInvocation ()
		{
			return "hudsub";
		}

		public override int execute (params string[] args)
		{
			GameObject inst = GameObject.Find (args [1]);
			if (inst == null)
				throw new ExecutionException ("No GO with that name exists!");
			
			Entity subject = inst.GetComponent<Entity> ();
			if (subject == null)
				throw new ExecutionException ("Target is not an Entity!");

			try
			{
				HUDManager.GetInstance().SetSubject (subject);
			}
			#pragma warning disable 0168
			catch(System.NullReferenceException nre)
			#pragma warning restore 0168
			{
				throw new ExecutionException ("There is no currently active HUD!");
			}

			Console.println ("HUD is now pulling data from " + subject.name + ".");
			return Console.EXEC_SUCCESS;
		}
	}
}
