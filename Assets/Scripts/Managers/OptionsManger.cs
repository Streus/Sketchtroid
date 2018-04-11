using System;
using System.Runtime.Serialization;
using UnityEngine;

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

		//set default bindings
		bindings.setBinding(Bindings.C_FORWARD, KeyCode.W);
		bindings.setBinding(Bindings.C_LEFT, KeyCode.A);
		bindings.setBinding(Bindings.C_DOWN, KeyCode.S);
		bindings.setBinding(Bindings.C_RIGHT, KeyCode.D);
		bindings.setBinding(Bindings.C_ABIL_1, KeyCode.Mouse0);
		bindings.setBinding(Bindings.C_ABIL_2, KeyCode.Mouse1);
		bindings.setBinding(Bindings.C_ABIL_3, KeyCode.Space);
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
