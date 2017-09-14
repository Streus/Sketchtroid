using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneDoor : MonoBehaviour
{
	private Collider2D target;
	private bool turning = true;

	[SerializeField]
	private float borderPadding = 3f;

	[SerializeField]
	private string targetRoom;

	public void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag != "Player")
			return;

		//unhook the camera
		CameraManager.scene_cam.isFollowingTarget = false;

		//disable player control
		col.GetComponent<Controller> ().enabled = false;

		//save values to instrument player velocity
		target = col;
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

			StartCoroutine (fadeScreenOut(HUDManager.instance.getFadeMask ()));

			Vector2 screenPos = Camera.main.WorldToScreenPoint (target.transform.position);
			if (screenPos.x > Screen.width + borderPadding || screenPos.x < -borderPadding
			    || screenPos.y > Screen.height + borderPadding || screenPos.y < -borderPadding)
				SceneStateManager.instance ().transitionTo (targetRoom);
		}
	}

	//screen fade coroutine
	private IEnumerator fadeScreenOut(Image fade)
	{
		while (fade.color.a < 1f)
		{
			float alpha = fade.color.a + Time.deltaTime / 2f;
			fade.color = new Color (fade.color.r, fade.color.g, fade.color.b, alpha);
			yield return null;
		}
	}
}
