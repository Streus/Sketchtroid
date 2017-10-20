using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Controller
{
	/* Instance Vars */

	/* Instance Methods */
	public override void Awake ()
	{
		base.Awake ();
	}

	// Inject some test data into self
	public void Start()
	{
		
	}

	public override void Update()
	{
		if (Console.log.isEnabled)
			return;

		//invoke abilities
		if (Input.GetKey (KeyCode.Mouse0)) //TODO swap for proper bindings later
			useAbility (0, Vector2.zero, self.defaultDT);
		if (Input.GetKey (KeyCode.Mouse1)) //TODO swap for proper bindings later
			useAbility (1, Vector2.zero);
		if (Input.GetKey (KeyCode.Space)) //TODO swap for proper bindings later
			useAbility (2, Vector2.zero);

		//TODO better way of interacting things?
		if (Input.GetKeyDown (KeyCode.E))
		{
			RaycastHit2D[] hits;
			hits = Physics2D.CircleCastAll (transform.position, 1f, Vector2.zero);

			foreach(RaycastHit2D hit in hits)
			{
				Interactable i = hit.collider.GetComponent<Interactable> ();
				if (i != null)
					i.OnInteract ();
			}
		}

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
