using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Controller
{
	/* Instance Vars */
	private Bindings binds;

	/* Instance Methods */
	public override void Awake ()
	{
		base.Awake ();
		binds = OptionsManger.getInstance ().getBindings ();
	}

	// Inject some test data into self
	public void Start()
	{
		//DEBUG add an infinite status
		self.addStatus (Status.get ("DEBUG", float.PositiveInfinity));
	}

	public override void Update()
	{
		if (!physbody.simulated)
			return;

		//invoke abilities
		if (binds.getControl(Bindings.C_ABIL_1))
			useAbility (0, Vector2.zero, self.defaultDT);
		if (binds.getControl(Bindings.C_ABIL_2))
			useAbility (1, Vector2.zero);
		if (binds.getControl(Bindings.C_ABIL_3))
			useAbility (2, Vector2.zero);

		//TODO bring up damage type selector a-la TE ?
		if (Input.GetKeyDown (KeyCode.Q))
		{
			DamageType next = self.defaultDT;
			for (int i = 1; i < 7; i++)
			{
				next = (DamageType) (((int)self.defaultDT + i) % 7);

				if (GameManager.instance.isDTUnlocked (next))
					break;
			}

			if (next != DamageType.NONE)
				self.defaultDT = next;
		}

		//DEBUG unlocking damage types
		if (Input.GetKeyDown (KeyCode.Alpha1))
			GameManager.instance.unlockDT (DamageType.ELECTRIC);
		if (Input.GetKeyDown (KeyCode.Alpha2))
			GameManager.instance.unlockDT (DamageType.BIO);
		if (Input.GetKeyDown (KeyCode.Alpha3))
			GameManager.instance.unlockDT (DamageType.CRYO);
		if (Input.GetKeyDown (KeyCode.Alpha4))
			GameManager.instance.unlockDT (DamageType.PYRO);
		if (Input.GetKeyDown (KeyCode.Alpha5))
			GameManager.instance.unlockDT (DamageType.VOID);
	}

	public override void FixedUpdate()
	{
		if (!physbody.simulated)
			return;

		//face the mouse
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
		facePoint (mousePos);

		//movement
		Vector2 movementVector = Vector2.zero;

		bool up = binds.getControl (Bindings.C_FORWARD);
		bool left = binds.getControl (Bindings.C_LEFT);
		bool down = binds.getControl (Bindings.C_DOWN);
		bool right = binds.getControl (Bindings.C_RIGHT);

		if (up)
			movementVector += Vector2.up;
		if (left)
			movementVector += Vector2.left;
		if (down)
			movementVector += Vector2.down;
		if (right)
			movementVector += Vector2.right;

		physbody.AddForce (movementVector * self.movespeed.value);
	}
}
