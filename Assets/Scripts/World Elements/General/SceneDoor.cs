using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneDoor : MonoBehaviour
{
	/* Static Vars */
	private List<SceneDoor> doors;

	/* Instance Vars */
	private Collider2D target;
	private bool turning = true;

	[SerializeField]
	private float borderPadding = 3f;

	[SerializeField]
	private string destination;

	[SerializeField]
	private GameObject spawnPosition;
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

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag != "Player")
			return;

		if (!transitioningIn)
		{
			//unhook the camera
			CameraManager.scene_cam.isFollowingTarget = false;

			//disable player control
			col.GetComponent<Controller> ().enabled = false;

			//save values to instrument player velocity
			target = col;
		}
		else
		{
			CameraManager.scene_cam.setTarget (col.transform);
			CameraManager.scene_cam.isFollowingTarget = true;
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

			HUDManager.instance.fade (0.5);

			Vector2 screenPos = Camera.main.WorldToScreenPoint (target.transform.position);
			if (screenPos.x > Screen.width + borderPadding || screenPos.x < -borderPadding
			    || screenPos.y > Screen.height + borderPadding || screenPos.y < -borderPadding)
				SceneStateManager.instance ().transitionTo (destination);
		}
	}

	public void startTransitionIn(GameObject player)
	{
		transitioningIn = true;
		player.transform.position = spawnPosition.transform.position;
		player.transform.rotation = Quaternion.Euler (0f, 0f, transform.rotation.eulerAngles.z - 180f);
		player.GetComponent<Controller> ().enabled = false;

		target.GetComponent<Rigidbody2D> ().AddForce (target.transform.up * 8f, ForceMode2D.Impulse);

		HUDManager.instance.fade (-0.5);
	}
}
