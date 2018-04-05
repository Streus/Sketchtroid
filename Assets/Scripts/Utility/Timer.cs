using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

[System.Serializable]
public class Timer : ISerializable
{
	#region INSTANCE_VARS

	// The current value of this timer 
	private float value;

	// The maximum value
	private float max;

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
	public bool tick(float delta)
	{
		return (value = Mathf.Min(value + delta, max)) >= max;
	}

	public float getValue()
	{
		return value;
	}

	public float getMax()
	{
		return max;
	}

	/// <summary>
	/// Sets the value back to zero
	/// </summary>
	public void reset()
	{
		value = 0f;
	}

	/// <summary>
	/// Checks if this Timer is paused.
	/// </summary>
	public bool isPaused()
	{
		return paused;
	}

	/// <summary>
	/// Registers a pause
	/// </summary>
	public void setPause(bool val)
	{
		paused = val;
	}

	/// <summary>
	/// Returns the value as a percentage of the max (0-1)
	/// </summary>
	public float getCompletionPerc()
	{
		return value / max;
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
