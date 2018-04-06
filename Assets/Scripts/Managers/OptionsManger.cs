using System;
using System.Runtime.Serialization;

/// <summary>
/// Handles loading, saving, and managing global game options,
/// like audio levels, key bindings, console options, etc.
/// </summary>
public sealed class OptionsManger : ISerializable
{
	#region STATIC_VARS

	private static OptionsManger instance;
	#endregion

	#region INSTANCE_VARS

	// All the game relevent bindings
	private Bindings bindings;
	#endregion

	#region STATIC_METHODS

	public static OptionsManger getInstance()
	{
		if (instance == null)
		{
			instance = new OptionsManger ();
		}
		return instance;
	}
	#endregion

	#region INSTANCE_METHODS

	private OptionsManger()
	{
		bindings = new Bindings ();
	}
	public OptionsManger(SerializationInfo info, StreamingContext context)
	{
		bindings = (Bindings)info.GetValue ("bindings", typeof(Bindings));

		//overwrite old instance
		instance = this;
	}

	/// <summary>
	/// Gets the Bindings object managed by this instance
	/// </summary>
	public Bindings getBindings()
	{
		return bindings;
	}

	public void GetObjectData (SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("bindings", bindings);
	}
	#endregion
}
