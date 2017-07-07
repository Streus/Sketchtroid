using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
	// Returns help text for this command
	public abstract string getHelp();

	// Returns the string used to invoke this command
	public abstract string getInvocation();

	// Perform a function and return a string output
	public abstract string execute(params string[] args);

	public class ExecutionException : System.Exception
	{
		public ExecutionException(string message) : base(message) { }
	}
}
