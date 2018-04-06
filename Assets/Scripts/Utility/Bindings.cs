using UnityEngine;
using System.Runtime.Serialization;

/// <summary>
/// Holds all the game-relevent key and mouse bindings.
/// </summary>
public class Bindings : ISerializable //TODO Bindings class
{
	#region STATIC_VARS

	// Correspond to indexes in keycode array
	public const int 
	C_FORWARD = 0,
	C_LEFT = 1,
	C_DOWN = 2,
	C_RIGHT = 3;
	//TODO expand bindings list

	// Length of the bindings list
	private const int length = 4;

	#endregion

	#region INSTANCE_VARS

	private KeyCode[] keys;
	#endregion

	#region STATIC_METHODS

	#endregion

	#region INSTANCE_METHODS

	public Bindings()
	{
		keys = new KeyCode[length];
	}
	public Bindings(SerializationInfo info, StreamingContext context)
	{
		
	}

	/// <summary>
	/// Sets the key for the associated control
	/// </summary>
	public void setBinding(int control, KeyCode key)
	{
		try
		{
			keys[control] = key;
		}
		catch(System.IndexOutOfRangeException)
		{
			Debug.LogError ("Invalid control: " + control);
		}
	}

	/// <summary>
	/// Checks if the key associated with the control is in the given event.
	/// Returns true if it is, false otherwise.
	/// </summary>
	public bool getControl(int control, EventType type = EventType.Used)
	{
		if (control < 0 || control > length)
		{
			Debug.LogError ("Control index out of bounds: " + control);
			return false;
		}

		switch (type)
		{
		case EventType.used:
			return Input.GetKey (keys [control]);
		case EventType.keyDown:
			return Input.GetKeyDown (keys [control]);
		case EventType.keyUp:
			return Input.GetKeyUp (keys [control]);
		default:
			Debug.LogError (type.ToString () + " is not a valid event for controls.");
			return false;
		}
	}

	public void GetObjectData (SerializationInfo info, StreamingContext context)
	{
		throw new System.NotImplementedException ();
	}
	#endregion

	#region INTERNAL_TYPES

	#endregion
}
