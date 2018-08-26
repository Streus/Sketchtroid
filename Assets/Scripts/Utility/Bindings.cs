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
	C_RIGHT = 3,
	C_ABIL_1 = 4,
	C_ABIL_2 = 5,
	C_ABIL_3 = 6;
	//TODO expand bindings list

	// Length of the bindings list
	public const int length = 7;

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
		keys = (KeyCode[])info.GetValue ("keys", typeof(KeyCode[]));
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
		case EventType.Used:
			return Input.GetKey (keys [control]);
		case EventType.KeyDown:
			return Input.GetKeyDown (keys [control]);
		case EventType.KeyUp:
			return Input.GetKeyUp (keys [control]);
		default:
			Debug.LogError (type.ToString () + " is not a valid event for controls.");
			return false;
		}
	}

	public void GetObjectData (SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("keys", keys);
	}
	#endregion

	#region INTERNAL_TYPES

	#endregion
}
