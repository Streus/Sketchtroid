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
		self.AddStatus (Status.Get ("DEBUG", float.PositiveInfinity));
	}

	public override void Update()
	{
		if (!physbody.simulated)
			return;

		//invoke abilities
		if (binds.GetControl(Bindings.C_ABIL_1))
			UseAbility (0, Vector2.zero, self.DefaultDT);
		if (binds.GetControl(Bindings.C_ABIL_2))
			UseAbility (1, Vector2.zero);
		if (binds.GetControl(Bindings.C_ABIL_3))
			UseAbility (2, Vector2.zero);

		//TODO bring up damage type selector a-la TE ?
		if (Input.GetKeyDown (KeyCode.Q))
		{
			DamageType next = self.DefaultDT;
			for (int i = 1; i < 7; i++)
			{
				next = (DamageType) (((int)self.DefaultDT + i) % 7);

				if (GameManager.instance.isDTUnlocked (next))
					break;
			}

			if (next != DamageType.NONE)
				self.DefaultDT = next;
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
		FacePoint (mousePos);

		//movement
		Vector2 movementVector = Vector2.zero;

		bool up = binds.GetControl (Bindings.C_FORWARD);
		bool left = binds.GetControl (Bindings.C_LEFT);
		bool down = binds.GetControl (Bindings.C_DOWN);
		bool right = binds.GetControl (Bindings.C_RIGHT);

		if (up)
			movementVector += Vector2.up;
		if (left)
			movementVector += Vector2.left;
		if (down)
			movementVector += Vector2.down;
		if (right)
			movementVector += Vector2.right;

		physbody.AddForce (movementVector * self.movespeed.Value);
	}
}
