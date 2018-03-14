using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using System.IO;

public class Console : MonoBehaviour
{
	#region STATIC_VARS

	private const string INPUT = "i", OUTPUT = "o";

	// Channel constants
	public const int 
	MUTE = 0x0,
	DEFAULT = 0x1,
	BROADCAST = ~0x0;

	public static Console log;

	// Tags that are appended to print calls
	private static string[] tags = new string[]
	{
		"",
		"<color=#ff1111ff><b>[ERR]</b></color>", //red
		"<color=#ffff11ff><b>[WRN]</b></color>", //yellow
		"<color=#11ffffff><b>[INF]</b></color>", //cyan
		"<color=#11ff11ff><b>[OUT]</b></color>" //green
	};
	#endregion

	#region INSTANCE_VARS

	// Currently viewed channel in the GUI
	private int currentChannel;

	// The current value of the input field
	private string input;

	// The current values of the output fields
	private string[] channels;

	// The displayed names of all channels
	[SerializeField]
	private string[] channelNames = new string[sizeof(int) * 8];

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
	private Dictionary<string, Command> commands;

	// A list of all active variables
	private Dictionary<string, Variable> variables;

	// The current scope
	private int currScope;

	public Command[] getCommandList()
	{
		Command[] cs = new Command[commands.Values.Count];
		commands.Values.CopyTo(cs, 0);
		return cs;
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
	#endregion

	#region STATIC_METHODS

	// Set the name for a channel
	public static void setChannelName(int channel, string name)
	{
		if (channel == 0)
		{
			Debug.LogError ("Channel 1 is reserved!");
			return;
		}

		try { log.channelNames [channel] = name; }
		catch(System.IndexOutOfRangeException ioore)
		{
			Debug.LogException (ioore);
		}
	}

	public static int nameToChannel(string name)
	{
		if (name == "")
			throw new System.InvalidOperationException ("Cannot resolve empty string to channel.");

		for(int i = 0; i < log.channelNames.Length; i++)
		{
			if (log.channelNames [i] == name)
				return (1 << i); //return OR-able mask
		}

		//if the channel does not exist, should be muted
		return MUTE;
	}

	// Print a message to the console
	public static void println(string message, int channelMask = DEFAULT)
	{
		print (message + "\n", channelMask);
	}
	public static void println(string message, Tag tag, int channelMask = DEFAULT)
	{
		print (message + "\n", tag, channelMask);
	}
	public static void print(string message, int channelMask = DEFAULT)
	{
		print (message, Tag.none, channelMask);
	}
	public static void print(string message, Tag tag, int channelMask = DEFAULT)
	{
		if (log == null)
			return;

		channelMask |= DEFAULT;

		//update all channels in the mask
		for(int i = 0, c = sizeof(int) * 8; i < c; i++)
		{
			if ((channelMask & (1 << i)) != 0)
			{
				log.channels [i] += tags [(int)tag] + " " + message;
			}
		}

		//if the currently viewed channel was updated, update the GUI
		if ((channelMask & (1 << log.currentChannel)) != 0)
		{
			float outh = log.textStyle.CalcHeight (new GUIContent (log.channels [log.currentChannel]), Screen.width);
			log.scrollPos = new Vector2 (log.scrollPos.x, outh);
		}
	}

	// Clear the console output window
	public static void clear()
	{
		if(log != null)
			log.channels[log.currentChannel] = "";
	}

	// Maximize the console window
	public static void maximize()
	{
		if (log != null)
		{
			float inh = log.textStyle.CalcHeight (new GUIContent (log.input), Screen.width);
			log.outputFieldSize = Screen.height - inh;
		}
	}

	// Minimize the console window
	public static void minimize()
	{
		if(log != null)
			log.outputFieldSize = log.defaultOFSize;
	}
	#endregion

	#region INSTANCE_METHODS
	public void Awake()
	{
		if (log == null) 
		{
			log = this;
			DontDestroyOnLoad (gameObject);

			input = "";

			channels = new string[channelNames.Length];
			channelNames [0] = "Default";

			scrollPos = Vector2.zero;
			outputFieldSize = defaultOFSize;

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
		commands = new Dictionary<string, Command> ();

		putInCL (new Commands.Clear ());
		putInCL (new Commands.DumpSSM ());
		putInCL (new Commands.EndScope ());
		putInCL (new Commands.Execute ());
		putInCL (new Commands.Exit ());
		putInCL (new Commands.Help ());
		putInCL (new Commands.Hide ());
		putInCL (new Commands.Jump ());
		putInCL (new Commands.Maximize ());
		putInCL (new Commands.Minimize ());
		putInCL (new Commands.ModAbilities ());
		putInCL (new Commands.Print ());
		putInCL (new Commands.Save ());
		putInCL (new Commands.Set ());
		putInCL (new Commands.SetDifficulty ());
		putInCL (new Commands.SetHUDSub ());
		putInCL (new Commands.StartScope ());
		putInCL (new Commands.TestParse ());
	}
	private void putInCL(Command c)
	{
		commands.Add (c.getInvocation (), c);
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

		GUIContent outText = new GUIContent (channels[currentChannel]);
		GUIContent inText = new GUIContent (input);

		//TODO channel select
		float csw = 120f;
		currentChannel = GUI.SelectionGrid(
			new Rect(Screen.width - csw, 0, csw, Screen.height),
			currentChannel,
			channelNames, 1);

		float oth = textStyle.CalcHeight (outText, Screen.width - csw);
		float ith = textStyle.CalcHeight (inText, Screen.width - csw);

		scrollPos = GUI.BeginScrollView (
			new Rect (0, 0, Screen.width - csw, outputFieldSize), 
			scrollPos, 
			new Rect (0, 0, Screen.width - csw, oth));
		GUI.SetNextControlName (OUTPUT);
		GUI.Label (new Rect (0, 0, Screen.width - csw, oth), outText, textStyle);
		GUI.EndScrollView ();

		GUI.SetNextControlName (INPUT);
		input = GUI.TextField (new Rect (0, outputFieldSize, Screen.width - csw, ith), input, textStyle);
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

	#region CSER_STUFF
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
			println ("Could not load CSER file: " + fileName, Tag.error);
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

		string[] optionsLine = parseLine (commSer [0]);
		foreach (string option in optionsLine)
		{
			
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
					println ("Malformed goto on line " + i, Tag.error);
					return false;
				}
				int line = 0;
				if (lineMarkers.TryGetValue (tag, out line))
					i = line;
			}
			else //execute the command on the current line
				execute (commSer [i]);
		}

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

		Command c;
		if(commands.TryGetValue(args[0], out c))
		{
			try
			{
				commOut = c.execute (args);
			}
			catch(Command.ExecutionException ee)
			{
				println (ee.Message, Tag.error);
			}
			#pragma warning disable 0168
			catch(System.IndexOutOfRangeException ioore)
			#pragma warning restore 0168
			{
				println ("Provided too few arguments.\n" + c.getHelp(), Tag.error, BROADCAST);
			}

			if (commOut != "")
				println (commOut, Tag.command_out, BROADCAST);

			success = true;
		}
		if(!success)
			println ("Command not found.  Try \"help\" for a list of commands", Tag.error, BROADCAST);
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
		
		println (name + " does not exist in the current context.", Tag.error);
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
			println ("Cannot close global scope.", Tag.error);
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
	#endregion
	#endregion

	#region INTERNAL_TYPES

	// Used to tag output with appending strings and colors
	public enum Tag
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

	#endregion
}
