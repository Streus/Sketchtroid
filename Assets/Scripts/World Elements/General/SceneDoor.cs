using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour
{
	/* Static Vars */
	private static List<SceneDoor> doors;
	static SceneDoor()
	{
		doors = new List<SceneDoor>();
	}

	/* Instance Vars */
	private Collider2D target;
	private bool turning = true;

	[SerializeField]
	private float borderPadding = 3f;

	[SerializeField]
	private string destination;

	private bool transitioningIn = false;

	/* Static Methods */
	public static SceneDoor getDoor(string destination)
	{
		return doors.Find (delegate(SceneDoor obj) {
			return obj.destination == destination;
		});
	}

	/* Instance Methods */
	public void Awake()
	{
		doors.Add (this);
	}

	public void OnDestroy()
	{
		doors.Remove (this);
	}

	public void OnTriggerStay2D(Collider2D col)
	{
		if (col.GetComponent<Player>() == null)
			return;

		if (!transitioningIn)
		{
			//handle edge case where this is the 'first' door
			if (destination == "")
			{
				GetComponent<Collider2D> ().isTrigger = false;
				return;
			}

			//unhook the camera
			CameraManager.scene_cam.isFollowingTarget = false;

			//disable player control
			col.GetComponent<Controller> ().enabled = false;

			//save values to instrument player velocity
			target = col;
		}
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (col.GetComponent<Player>() == null)
			return;

		if (transitioningIn)
		{
			CameraManager.scene_cam.setTarget (col.transform);
			col.GetComponent<Controller> ().enabled = true;
		}
	}

	public void OnTriggerExit2D(Collider2D col)
	{
		if (transitioningIn)
			transitioningIn = false;
	}

	public void Update()
	{
		if (target == null)
			return;

		if (turning)
		{
			//turn align with "door"
			Vector3 finalDir = transform.up;
			Vector3 currentDir = target.transform.up;

			float dp = Vector3.Dot (finalDir, currentDir);
			float a = dp / (finalDir.magnitude * currentDir.magnitude);
			float angleTo = Mathf.Rad2Deg * Mathf.Acos (a);

			if (angleTo > 5f)
				target.transform.rotation = Quaternion.Lerp (target.transform.rotation, transform.rotation, Time.deltaTime * 3f);
			else
				turning = false;
		}
		else
		{
			//accelerate off screen, triggering a transition when fully off screen
			target.GetComponent<Rigidbody2D> ().AddForce (target.transform.up * 2f, ForceMode2D.Impulse);

			Vector2 screenPos = Camera.main.WorldToScreenPoint (target.transform.position);
			if (screenPos.x > Screen.width + borderPadding || screenPos.x < -borderPadding
			    || screenPos.y > Screen.height + borderPadding || screenPos.y < -borderPadding)
				SceneStateManager.instance ().transitionTo (destination);
		}
	}

	public void startTransitionIn(GameObject player)
	{
		transitioningIn = true;
		player.GetComponent<Controller> ().enabled = false;
		Transform ct = CameraManager.scene_cam.transform;
		ct.position = new Vector3 (transform.position.x, transform.position.y, ct.position.z);

		//calculate the position to place the player for the transition in
		Vector3 spawnPos = Camera.main.WorldToScreenPoint (transform.position);
		spawnPos += transform.up * new Vector3 (Screen.width, Screen.height/2f, 0f).magnitude;
		player.transform.position = Camera.main.ScreenToWorldPoint (spawnPos);

		//rotate to face door, disable player control, and propell them to the door
		player.transform.rotation = Quaternion.Euler (0f, 0f, transform.rotation.eulerAngles.z - 180f);
		player.GetComponent<Controller> ().enabled = false;

		player.GetComponent<Rigidbody2D> ().AddForce (player.transform.up * 60f, ForceMode2D.Impulse);

		//show current scene name
		HUDManager.instance.displayTextPrompt (SceneManager.GetActiveScene ().name, "");
	}
}
