using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using System.IO;

public class Console : MonoBehaviour
{
	/* Static Vars */
	public static Console log;

	private static Assembly assembly = Assembly.GetExecutingAssembly();

	// Tags that are appended to print calls
	private static string[] tags = new string[]
	{
		"",
		"<color=#ff1111ff><b>[ERROR]</b></color>",
		"<color=#ffff11ff><b>[WARN]</b></color>",
		"<color=#11ffffff><b>[INFO]</b></color>",
		"<color=#11ff11ff><b>[OUT]</b></color>"
	};

	/* Instance Vars */

	// The input line for the Console
	[SerializeField]
	private InputField input;

	// The output display for the Console
	[SerializeField]
	private Text output;

	// The base RectTransform of the Console
	[SerializeField]
	private RectTransform root;

	// The Console's Canvas Group
	[SerializeField]
	private CanvasGroup _canvas;

	// The maximum number of lines output will display (also history size)
	public int linesMax;

	// A list of all available commands
	private List<Command> commands;

	// A list of all active variables
	private Dictionary<string, Variable> variables;

	// The current scope
	private int currScope;

	public Command[] getCommandList()
	{
		return commands.ToArray ();
	}

	// Every individual 
	private List<string> history;
	private int historyIndex;

	private bool _enabled;
	public bool isEnabled
	{
		get { return _enabled; }
		set
		{
			_enabled = _canvas.interactable = _canvas.blocksRaycasts = value;
			_canvas.alpha = _enabled ? 1f : 0f;
			if (!_enabled)
			{
				input.text = "";
				input.DeactivateInputField ();
			}
			else
				input.ActivateInputField ();
		}
	}

	public bool isFocused
	{
		get { return input.isFocused; }
	}

	/* Instance Methods */
	public void Awake()
	{
		if (log == null) 
		{
			log = this;
			DontDestroyOnLoad (gameObject);

			commands = new List<Command> ();
			buildCommandList ();

			variables = new Dictionary<string, Variable> ();
			currScope = -1;

			history = new List<string> ();
			historyIndex = -1;

			isEnabled = false;
		}
		else
			Destroy (gameObject);
	}
	private void buildCommandList()
	{
		string baseDir = Directory.GetCurrentDirectory ();
		string[] files = Directory.GetFiles (baseDir + "/Assets/Scripts/Testing/Commands");
		foreach (string file in files)
		{
			if (file.EndsWith (".cs"))
			{
				int start = 1 + Mathf.Max (file.LastIndexOf ('/'), file.LastIndexOf ('\\'));
				int end = file.LastIndexOf ('.');

				string rawClass = file.Substring (start, end - start);

				Command c = (Command)assembly.CreateInstance ("Commands." + rawClass);
				if (c == null)
					Debug.LogError ("Non-Command file in Commands folder."); //DEBUG
				commands.Add (c);
			}
		}
	}

	public void Start()
	{
		input.onEndEdit.AddListener (delegate{inputEntered();});
	}

	public void Update()
	{
		if (Input.GetKeyDown (KeyCode.BackQuote)) //TODO replace with dynamic keybinding
		{
			isEnabled = !isEnabled;
			return;
		}

		if (!isEnabled)
			return;

		if (!isFocused && (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.UpArrow)))
		{
			input.ActivateInputField ();
			input.Select ();
			input.text = "";
			historyIndex = -1;
		}

		if (Input.GetKeyDown (KeyCode.UpArrow))
		{
			historyIndex = (historyIndex + 1) % history.Count;
			input.text = history [historyIndex];
		}

		if (Input.GetKeyDown (KeyCode.DownArrow))
		{
			historyIndex--;
			if (historyIndex < 0)
				historyIndex = history.Count - 1;
			input.text = history [historyIndex];
		}
	}

	private void inputEntered()
	{
		if (!isEnabled || input.text == "")
			return;

		string commOut = "";

		//parse input
		string[] args = parseLine(input.text);
		args [0] = args [0].ToLower ();

		//search for a command that matches args[0] and exec it
		if (execute(args, out commOut))
		{
			if (commOut != "")
				println (commOut, LogTag.command_out);
		}
		else
			println ("Command not found.  Try \"help\" for a list of commands", LogTag.error);

		history.Insert (0, input.text);
		input.text = "";
	}

	// Parse an individual input line and return an array of args
	private string[] parseLine(string line)
	{
		string[] args = line.Split (' ');
		List<string> mergeList = new List<string> ();
		string mergeString = "";
		bool merging = false;
		for (int i = 0; i < args.Length; i++)
		{
			//argument merging
			bool endsWith = args [i].EndsWith ("\"");
			if (endsWith && merging)
			{
				mergeString += args [i];
				mergeList.Add (resolveVariable(mergeString.Replace("\"", "")));
				mergeString = "";
				merging = false;
			}
			else if (args [i].StartsWith ("\"") || merging)
			{
				if (endsWith)
				{
					mergeString = "";
					mergeList.Add(resolveVariable(args[i].Replace("\"", "")));
				}
				else
					mergeString += args [i] + " ";
				merging = !endsWith;
			}
			else
			{
				mergeList.Add (resolveVariable(args [i]));
			}
		}
		return mergeList.ToArray ();
	}
	private string resolveVariable(string str)
	{
		int startInd = 0, endInd = 0;
		string resultStr = "";
		while (true)
		{
			startInd = str.IndexOf ('%', endInd);
			endInd = Mathf.Min (str.IndexOf (' '), str.IndexOf ('\"'), str.Length - 1);
			string resolution = getVariableValue(str.Substring (startInd, endInd - startInd));
		}

		return str;
	}

	// Attempt to execute a command. Fills output with the result of a successful command
	private bool execute(string[] command, out string output)
	{
		output = "";
		foreach (Command c in commands)
		{
			if(c.getInvocation().Equals(command[0]))
			{
				try
				{
					output = c.execute (command);
				}
				catch(Command.ExecutionException ee)
				{
					println (ee.Message, LogTag.error);
				}
				return true;
			}
		}
		return false;
	}

	// Create a variable and store it in the console's dictionary
	public bool declareVariable(string name, string value, bool globalVar = false)
	{
		int scope = globalVar ? -1 : currScope;
		if (!variables.ContainsKey (scope.ToString() + name))
			variables.Add (scope.ToString() + name, new Variable (name, value, scope));
		else
			return false;
		return true;
	}

	// Set the value of a variable currently managed by the console
	public bool setVariableValue(string name, string value, bool globalVar = false)
	{
		Variable v = null;
		if (globalVar && variables.TryGetValue ("-1" + name, out v))
		{
			v.value = value;
			return true;
		}

		for(int i = currScope; i >= -1; i--)
		{
			if(variables.TryGetValue (i.ToString() + name, out v))
			{
				v.value = value;
				return true;
			}
		}
		return false;
	}

	// Get the value of a variable via its name
	private string getVariableValue(string name, bool globalVar = false)
	{
		Variable v = null;

		if (globalVar && variables.TryGetValue ("-1" + name, out v))
			return v.value;
		
		for(int i = currScope; i >= -1; i--)
			if(variables.TryGetValue (i.ToString() + name, out v))
				return v.value;
		
		println (name + " does not exist in the current context.", LogTag.error);
		return name;
	}

	// Open new scope
	public void openScope()
	{
		currScope++;
	}

	// Close the current scope, if it's not the global scope
	public bool closeScope()
	{
		if (currScope == -1)
		{
			println ("Cannot close global scope.", LogTag.error);
			return false;
		}

		// Remove all variables in the current scope
		foreach (Variable v in variables.Values)
		{
			if (v.scope == currScope)
				variables.Remove (v.name);
		}

		currScope--;
		return true;
	}

	// Return a summary string for the console's variable dictionary
	public string dumpVariables()
	{
		string str = "Current;y managing " + variables.Count + " variables";
		foreach (Variable v in variables.Values)
			str += "\n[" + v.scope + "] " + v.name + " = " + v.value;
		return str;
	}

	// Print a message to the console
	public void println(string message)
	{
		print (message + "\n");
	}
	public void println(string message, LogTag tag)
	{
		print (message + "\n", tag);
	}
	public void print(string message)
	{
		print (message, LogTag.none);
	}
	public void print(string message, LogTag tag)
	{
		output.text += tags [(int)tag] + " " + message;

		if (output.cachedTextGenerator.lineCount > linesMax)
		{
			string currOutput = output.text;
			output.text = currOutput.Substring (output.cachedTextGenerator.lines [1].startCharIdx);
		}
	}

	// Clear the console output window
	public void clear()
	{
		output.text = "";
	}

	// Maximize the console window
	public void maximize()
	{
		root.anchorMin = new Vector2 (0f, 0f);
		root.pivot = new Vector2 (0.5f, 0.5f);
		root.sizeDelta = new Vector2 (0f, 0f);
	}

	// Minimize the console window
	public void minimize()
	{
		root.anchorMin = new Vector2 (0f, 1f);
		root.pivot = new Vector2 (0.5f, 1f);
		root.sizeDelta = new Vector2 (0f, 100f);
	}

	/* Inner Classes */

	// Used to tag output with appending strings and colors
	public enum LogTag
	{
		none, error, warning, info, command_out
	}

	// To be used to save and retrive values in commands and .cser files
	private class Variable
	{
		// The name used to reference the variable
		public readonly string name;

		// The value the variable holds
		public string value;

		// The scope this variable is part of. when this scope ends, the variable is discarded
		// A value of -1 makes it global
		public readonly int scope;

		public Variable(string name, string value, int scope = -1)
		{
			this.name = name;
			this.value = value;
			this.scope = scope;
		}
	}
}
