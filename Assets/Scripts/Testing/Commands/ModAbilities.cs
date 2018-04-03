using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
	public class ModAbilities : Command
	{
		public override string getHelp ()
		{
			return "Modify the ability set of the current HUD target. " +
				"Usage: abil [add <name>|rm <name|index>|swp <index> <name>]";
		}

		public override string getInvocation ()
		{
			return "abil";
		}

		public override int execute (params string[] args)
		{
			int curIndex = 1;
			string printOut = "ERROR";

			//pre-checks
			if (HUDManager.getInstance() == null)
				throw new ExecutionException ("No currently active HUD!");
			if (HUDManager.getInstance().getSubject () == null)
				throw new ExecutionException ("HUD has no subject!");

			Entity rep = HUDManager.getInstance().getSubject ();

			// no modifications, just print out ability list info
			if (args.Length < 2)
			{
				printOut = "Abilities[" + rep.abilityCount + "]:";
				for (int i = 0; i < rep.abilityCap; i++)
				{
					Ability a = rep.getAbility (i);
					if(a != null)
						printOut += "\n" + a.name;
					else
						printOut += "\n" + "|";
				}
				Console.println (printOut, Console.Tag.info);
				return Console.EXEC_SUCCESS;
			}

			//define action
			switch (args [curIndex])
			{
			//add an ability to the entity's set
			case "add":
				curIndex++;
				rep.addAbility (Ability.get (args [curIndex]));
				printOut = "Added " + args [curIndex] + " to " + rep.name
					+ "'s ability set.";
				break;

			//remove an ability or abilities from the entity's set
			case "rm":
				curIndex++;
				int index = 0;
				if (int.TryParse (args [curIndex], out index))
				{
					if (index >= rep.abilityCount)
						throw new ExecutionException ("Entity only has " + rep.abilityCount + "abilities! "
						+ index + " is out of bounds!");
					rep.removeAbility (index);
					printOut = "Removed ability #" + index + " from " + rep.name
						+ "'s ability set.";
				}
				else
				{
					for (int i = 0; i < rep.abilityCount; i++)
					{
						if (rep.getAbility (i).name == args [curIndex])
						{
							rep.removeAbility (i);
							break;
						}
					}
					printOut = "Removed " + args [curIndex] + " from " + rep.name
						+ "'s ability set.";
				}
				break;

			//swap an ability in the entity's set with a new ability
			case "swp":
				curIndex++;
				if (int.TryParse (args [curIndex], out index))
				{
					if (index >= rep.abilityCount)
						throw new ExecutionException ("Entity only has " + rep.abilityCount + "abilities! "
						+ index + " is out of bounds!");
					rep.swapAbility (Ability.get(args[++curIndex]), index);
					printOut = "Swapped ability #" + index + " with " + args[curIndex]
					+ " in " + rep.name + "'s ability set.";
				}
				else
					throw new ExecutionException (args [curIndex] + " should be an int!");
				break;
			}

			Console.println (printOut, Console.Tag.info);
			return Console.EXEC_SUCCESS;
		}
	}
}
