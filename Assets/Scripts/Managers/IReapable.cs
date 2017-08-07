using System.Runtime.Serialization;

public interface IReapable
{
	// Extract important values and return them in a serializable class
	SeedBase reap ();

	// Take a serializable class and attempt to use it to fill values
	void sow(SeedBase seed);

	// if true, the SSM will never reset this object back to its default values
	bool ignoreReset ();
}
