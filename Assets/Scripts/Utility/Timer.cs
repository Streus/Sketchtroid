using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

[System.Serializable]
public class Timer : ISerializable
{
	#region INSTANCE_VARS

	// The current value of this timer 
	[SerializeField]
	private float value;
	public float Value { get { return value; } }
	public float MirrorValue { get { return max - value; } }

	// The maximum value
	[SerializeField]
	private float max;
	public float Max { get { return Max; } }

	// Halts updating of the value if true
	private bool paused;
	#endregion

	#region INSTANCE_METHODS

	public Timer()
	{
		value = 0f;
		max = 1f;
		paused = false;
	}
	public Timer(float max)
	{
		value = 0f;
		this.max = max;
		paused = false;
	}
	public Timer(SerializationInfo info, StreamingContext context)
	{
		value = info.GetSingle ("value");
		max = info.GetSingle ("max");
		paused = info.GetBoolean ("paused");
	}

	/// <summary>
	/// Updates the value by delta.  Returns true if the value is greater than
	/// the max after the addition, false otherwise.
	/// </summary>
	public bool Tick(float delta)
	{
		return (value = Mathf.Min(value + delta, max)) >= max;
	}

	/// <summary>
	/// Returns true if the value is greater than the max, false otherwise
	/// </summary>
	public bool Check()
	{
		return value >= max;
	}

	/// <summary>
	/// Sets the value back to zero
	/// </summary>
	public void Reset()
	{
		value = 0f;
	}

	/// <summary>
	/// Checks if this Timer is paused.
	/// </summary>
	public bool IsPaused()
	{
		return paused;
	}

	/// <summary>
	/// Registers a pause
	/// </summary>
	public void SetPause(bool val)
	{
		paused = val;
	}

	/// <summary>
	/// Returns the value as a percentage of the max (0-1)
	/// </summary>
	public float GetCompletionPerc()
	{
		return value / max;
	}

	public float GetMirroredCompletionPerc()
	{
		return 1 - GetCompletionPerc ();
	}

	/// <summary>
	/// Used for serialization
	/// </summary>
	public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
	{
		info.AddValue("value", value);
		info.AddValue("max", max);
		info.AddValue ("paused", paused);
	}
	#endregion
}
