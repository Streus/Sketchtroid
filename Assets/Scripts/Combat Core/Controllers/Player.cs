using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Controller
{
	/* Instance Vars */


	public DamageType damageType = DamageType.NONE; //DEBUG ?

	/* Instance Methods */
	public override void Awake ()
	{
		base.Awake ();
	}

	// Inject some test data into self
	public void Start()
	{
		self.addAbility (Ability.get("Spray"));
	}

	public override void Update()
	{
		if (Console.log.isEnabled)
			return;

		//invoke abilities
		if (Input.GetKey (KeyCode.Mouse0)) //TODO swap for proper bindings later
			useAbility (0, Vector2.zero, damageType);
	}

	public override void FixedUpdate()
	{
		if (Console.log.isEnabled)
			return;

		//face the mouse
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
		facePoint (mousePos);

		//movement
		Vector2 movementVector = Vector2.zero;

		bool up = Input.GetKey (KeyCode.W); //TODO swap for proper bindings later
		bool left = Input.GetKey(KeyCode.A);
		bool down = Input.GetKey (KeyCode.S);
		bool right = Input.GetKey (KeyCode.D);

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
