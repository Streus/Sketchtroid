using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using System.IO;

public class Console : MonoBehaviour
{
	private const string INPUT = "input", OUTPUT = "output";

	/* Static Vars */
	public static Console log;

	private static Assembly assembly = Assembly.GetExecutingAssembly();

	// Tags that are appended to print calls
	private static string[] tags = new string[]
	{
		"",
		"<color=#ff1111ff><b>[ERROR]</b></color>", //red
		"<color=#ffff11ff><b>[WARN]</b></color>", //yellow
		"<color=#11ffffff><b>[INFO]</b></color>", //cyan
		"<color=#11ff11ff><b>[OUT]</b></color>" //green
	};

	/* Instance Vars */

	// The current value of the input field
	private string input;

	// The current value of the output field
	private string output;

	// Position of output field viewport
	private Vector2 scrollPos;

	// Size of output field viewport
	private float outputFieldSize;
	[SerializeField]
	private float defaultOFSize = 70f;

	// The style used by all GUI rendered by this console
	[SerializeField]
	private GUIStyle textStyle;
	[SerializeField]
	private Color backgroundColor = Color.black;

	// The maximum number of lines output will display (also history size)
	public int linesMax;

	// A list of all available commands
	private List<Command> commands;

	// A list of all active variables
	private Dictionary<string, Variable> variables;

	// The current scope
	private int currScope;

	// When true, command output will be suppressed
	private bool quietMode;

	public Command[] getCommandList()
	{
		return commands.ToArray ();
	}

	// Every individual user entry made during current runtime
	private List<string> history;
	private int historyIndex;

	private bool _enabled;
	public bool isEnabled
	{
		get { return _enabled; }
		set
		{
			_enabled = value;
			//manage pause states
			if (GameManager.instance != null)
			{
				if (GameManager.instance.isPaused ())
				{
					GameManager.instance.setPauseLock (false);
					GameManager.instance.setPause (false);
				}
				else
				{
					GameManager.instance.setPause (true);
					GameManager.instance.setPauseLock (true);
				}
			}
		}
	}

	public bool isFocused
	{
		get { return GUI.GetNameOfFocusedControl() == INPUT; }
	}

	/* Instance Methods */
	public void Awake()
	{
		if (log == null) 
		{
			log = this;
			DontDestroyOnLoad (gameObject);

			input = "";

			scrollPos = Vector2.zero;
			outputFieldSize = defaultOFSize;

			commands = new List<Command> ();
			buildCommandList ();

			variables = new Dictionary<string, Variable> ();
			currScope = -1;

			history = new List<string> ();
			historyIndex = -1;

			isEnabled = false;

			quietMode = false;
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
		
	}

	public void Update()
	{
		if (Input.GetKeyDown (KeyCode.BackQuote)) //TODO replace with dynamic keybinding
		{
			isEnabled = !isEnabled;
			return;
		}
	}

	public void OnGUI()
	{
		if (!isEnabled)
			return;

		Event e = Event.current;
		if (isFocused && e.isKey && e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)
			inputEntered ();

		//move focus to input line
		if (!isFocused && (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.UpArrow)))
		{
			GUI.FocusControl (INPUT);
			input = "";
			historyIndex = -1;
			return;
		}

		//navigate the history
		if (Input.GetKeyDown (KeyCode.UpArrow))
		{
			historyIndex = (historyIndex + 1) % history.Count;
			input = history [historyIndex];
		}
		if (Input.GetKeyDown (KeyCode.DownArrow))
		{
			historyIndex--;
			if (historyIndex < 0)
				historyIndex = history.Count - 1;
			input = history [historyIndex];
		}

		//draw GUI
		GUI.backgroundColor = backgroundColor;

		GUIContent outText = new GUIContent (output);
		GUIContent inText = new GUIContent (input);

		float oth = textStyle.CalcHeight (outText, Screen.width);
		float ith = textStyle.CalcHeight (inText, Screen.width);

		scrollPos = GUI.BeginScrollView (
			new Rect (0, 0, Screen.width, outputFieldSize), 
			scrollPos, 
			new Rect (0, 0, Screen.width, oth));
		GUI.SetNextControlName (OUTPUT);
		GUI.Label (new Rect (0, 0, Screen.width, oth), outText, textStyle);
		GUI.EndScrollView ();

		GUI.SetNextControlName (INPUT);
		input = GUI.TextField (new Rect (0, outputFieldSize, Screen.width, ith), input, textStyle);
	}

	// Invoked when the user presses enter and the console is active
	private void inputEntered()
	{
		if (!isEnabled || input.Length == 0)
			return;

		//use the text from input to execute a command
		execute(input);
		input = "";
	}

	// Execute a file of prepared commands, treating each line as an indiv. command
	public bool executeFile(string fileName)
	{
		string[] commSer = new string[0];

		//load file into a string array
		try
		{
			commSer = File.ReadAllLines (fileName);
		}
		#pragma warning disable 0168
		catch(IOException ioe)
		#pragma warning restore 0168
		{
			println ("Could not load CSER file: " + fileName, LogTag.error);
			return false;
		}

		//parse for line markers
		Dictionary<string, int> lineMarkers = new Dictionary<string, int> ();
		for (int i = 0; i < commSer.Length; i++)
		{
			if(commSer[i].StartsWith(":"))
			{
				int endTag = commSer [i].IndexOf (' ');
				if (endTag == -1)
					endTag = commSer [i].Length - 1;
				string tag = commSer [i].Substring (1, endTag - 1);
				if(tag != "")
					lineMarkers.Add (tag, i);
			}	
		}

		//save current option statuses for reset later
		bool quietMode_store = quietMode;

		string[] optionsLine = parseLine (commSer [0]);
		foreach (string option in optionsLine)
		{
			if (option == "-q")
				quietMode = true;
		}

		//execute the file
		for (int i = 0; ++i < commSer.Length;)
		{
			//goto handling
			if (commSer [i].StartsWith ("goto", false, System.Globalization.CultureInfo.CurrentCulture))
			{
				string tag;
				try
				{
					tag = commSer [i].Split (' ') [1];
				}
				#pragma warning disable 0168
				catch (System.IndexOutOfRangeException ioobe)
				#pragma warning restore 0168
				{
					println ("Malformed goto on line " + i, LogTag.error);
					return false;
				}
				int line = 0;
				if (lineMarkers.TryGetValue (tag, out line))
					i = line;
			}
			else //execute the command on the current line
				execute (commSer [i]);
		}

		//reset any toggled options
		quietMode = quietMode_store;

		return true;
	}

	// Attempt to execute a command. Fills output with the result of a successful command
	public bool execute(string command)
	{
		string commOut = "";
		bool success = false;

		//parse input
		string[] args = parseLine(command);
		args [0] = args [0].ToLower ();

		foreach (Command c in commands)
		{
			if(c.getInvocation().Equals(args[0]))
			{
				try
				{
					commOut = c.execute (args);
				}
				catch(Command.ExecutionException ee)
				{
					println (ee.Message, LogTag.error);
				}
				#pragma warning disable 0168
				catch(System.IndexOutOfRangeException ioore)
				#pragma warning restore 0168
				{
					println ("Provided too few arguments.\n" + c.getHelp(), LogTag.error);
				}

				if (commOut != "")
					println (commOut, LogTag.command_out);

				success = true;
			}
		}
		if(!success)
			println ("Command not found.  Try \"help\" for a list of commands", LogTag.error);
		history.Insert (0, input);

		return success;
	}

	// Parse an individual input line and return an array of args
	private string[] parseLine(string line)
	{
		List<string> argsList = new List<string> ();
		string mergeString = "";
		bool merging = false;

		for (int i = 0; i < line.Length; i++)
		{
			if (line [i] == '\"') //start or end space-ignoring group
			{
				merging = !merging;
				if (!merging)
				{
					argsList.Add (mergeString);
					mergeString = "";
				}
			}
			else if (line [i] == '%' || line [i] == '!') //try to resolve a variable
			{
				char delim = line [i] == '%' ? '%' : '!';
				int start = i + 1;
				int end = line.IndexOf (delim, start);
				if (end != -1)
				{
					mergeString += getVariableValue (line.Substring (start, end - start), line [i] == '!');
					i = end;
				}
			}
			else if (line [i] == ' ' && !merging) //end of a regular term
			{
				if(mergeString != "")
					argsList.Add (mergeString);
				mergeString = "";
			}
			else //add any other character to the mergeString
				mergeString += line [i];
		}

		//if the merge string is not empty, add it the the args list
		if(mergeString != "")
			argsList.Add (mergeString);

		//return the parsed result
		return argsList.ToArray ();
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
		string str = "Currently managing " + variables.Count + " variables";
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
		if (quietMode && tag == LogTag.command_out)
			return;
		
		output += tags [(int)tag] + " " + message;

		float outh = textStyle.CalcHeight (new GUIContent (output), Screen.width);
		scrollPos = new Vector2 (scrollPos.x, outh);
	}

	// Clear the console output window
	public void clear()
	{
		output = "";
	}

	// Maximize the console window
	public void maximize()
	{
		float inh = textStyle.CalcHeight (new GUIContent (input), Screen.width);
		outputFieldSize = Screen.height - inh;
	}

	// Minimize the console window
	public void minimize()
	{
		outputFieldSize = defaultOFSize;
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
