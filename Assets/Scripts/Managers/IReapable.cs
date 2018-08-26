
public interface IReapable
{
	// Extract important values and return them in a serializable class
	SeedCollection.Base Reap ();

	// Take a serializable class and attempt to use it to fill values
	void Sow(SeedCollection.Base seed);
}
