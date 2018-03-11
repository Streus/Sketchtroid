using System.Collections.Generic;

public partial class Status
{
	static Status()
	{
		repo = new Dictionary<string, Status> ();

		//template put
		put (new Status (
			"Empty",
			"An empty status that does nothing",
			"",
			DecayType.serial,
			1,
			5f));
	}
}