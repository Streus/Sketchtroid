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

	// The Console's Canvas Group
	[SerializeField]
	private CanvasGroup _canvas;

	// The maximum number of lines output will display (also history size)
	public int linesMax;

	// A list of all available commands
	private List<Command> commands;

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
			isEnabled = !isEnabled;

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
		string[] args = input.text.Split(' ', ',');
		args [0] = args [0].ToLower ();

		//search for a command that matches args[0]
		bool found = false;
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
				found = true;
				break;
			}
		}
		if (found)
			println (commOut, LogTag.command_out);
		else
			println ("Command not found.  Try \"help\" for a list of commands", LogTag.error);

		history.Insert (0, input.text);
		input.text = "";
	}

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

	// Used to tag output with appending strings and colors
	public enum LogTag
	{
		none, error, warning, info, command_out
	}
}
