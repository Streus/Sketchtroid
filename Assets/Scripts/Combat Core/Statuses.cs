using System.Collections.Generic;

public partial class Status
{
	static Status()
	{
		repo = new Dictionary<string, Status> ();

		//template put
		put (new Status (
			"DEBUG",
			"An empty status that does nothing, and lasts forever.",
			"",
			DecayType.serial,
			1,
			5f));
	}
}