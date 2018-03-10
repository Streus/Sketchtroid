using System.Runtime.Serialization;

public interface IReapable
{
	// Extract important values and return them in a serializable class
	SeedCollection.Base reap ();

	// Take a serializable class and attempt to use it to fill values
	void sow(SeedCollection.Base seed);
}
