﻿using System.Collections;
using System.Collections.Generic;

public struct Stat
{
	/* Instance Vars and Accessors */

	// The imutable inital value of this Stat
	private int baseValue;

	// Added onto the base
	private int addValue;

	// A multiplier applied to the base + add
	private float multiValue;

	// A value that can replace the calculated value
	private int lockValue;

	// Is this Stat locked to lockValue?
	public bool locked;

	// A floor cap for value
	private bool hasMin;
	private int minValue;

	// A ceiling cap for value
	private bool hasMax;
	private int maxValue;

	// The externally viewable value of this Stat
	public int value
	{
		get
		{
			if (locked)
				return lockValue;

			int tVal = calculatedValue;

			if (hasMin && tVal < minValue)
				return minValue;
			if (hasMax && tVal > maxValue)
				return maxValue;

			return tVal;
		}
	}

	// (bass + add) * mult
	private int calculatedValue
	{
		get
		{
			int aVal = baseValue + addValue;
			return (int)(((float)aVal) * multiValue);
		}
	}

	public int min
	{
		get
		{
			if (hasMin)
				return minValue;
			return int.MinValue;
		}
		set
		{
			minValue = min;
			hasMin = true;

			OnStatChanged ();
		}
	}
	public int max
	{
		get
		{
			if(hasMax)
				return maxValue;
			return int.MaxValue;
		}
		set
		{
			maxValue = max;
			hasMax = true;
			OnStatChanged ();
		}
	}

	/* Static Methods */
	public static Stat operator +(Stat s, int value)
	{
		s.addValue += value;
		s.OnStatChanged ();
		return s;
	}

	public static Stat operator -(Stat s, int value)
	{
		s.addValue -= value;
		s.OnStatChanged ();
		return s;
	}

	public static Stat operator *(Stat s, float value)
	{
		s.multiValue *= value;
		s.OnStatChanged ();
		return s;
	}

	public static Stat operator /(Stat s, float value)
	{
		s.multiValue /= value;
		s.OnStatChanged ();
		return s;
	}

	/* Constructor(s) */
	public Stat(int baseValue)
	{
		this.baseValue = baseValue;

		addValue = 0;

		multiValue = 1f;

		lockValue = 0;
		locked = false;

		hasMin = false;
		minValue = 0;

		hasMax = false;
		maxValue = 0;

		statModified = null;
	}
	public Stat(int baseValue, int minValue) : this(baseValue)
	{
		this.hasMin = true;
		this.minValue = minValue;
	}
	public Stat(int baseValue, int minValue, int maxValue) : this(baseValue)
	{
		this.hasMin = true;
		this.minValue = minValue;

		this.hasMax = true;
		this.maxValue = maxValue;
	}

	/* Instance Methods */

	// Modify the baseValue of this Stat
	public void setBase(int baseValue)
	{
		this.baseValue = baseValue;
		OnStatChanged ();
	}

	// Set the lockValue and indicate it should be used instead of the calculated value
	public void lockTo(int lockValue)
	{
		this.lockValue = lockValue;
		locked = true;

		OnStatChanged ();
	}

	// Indicate the calculated value should be used for now
	public void unlock()
	{
		locked = false;
		OnStatChanged ();
	}

	// Remove the maximum limit on this Stat
	public void removeMax()
	{
		hasMax = false;
		OnStatChanged ();
	}

	// Remove the maximum limit on this Stat
	public void removeMin()
	{
		hasMin = false;
		OnStatChanged ();
	}

	// Used for resource bar type applications
	public void maximize()
	{
		if(hasMax)
			addValue = maxValue;
	}
	public void minimize()
	{
		if (hasMin)
			addValue = minValue;
	}

	public delegate void ChangedStat(Stat s);
	public event ChangedStat statModified;
	private void OnStatChanged()
	{
		if (statModified != null)
			statModified (this);
	}
}