using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	/* Static Vars */
	public static CameraManager scene_cam;

	/* Instance Vars */

	// The Transform of the GameObject to which the camera is attached
	private Transform cam;

	// A queue of PathNodes to transition through
	private Queue<PathNode> path;

	// Target stuff
	public Transform target;
	public bool isFollowingTarget;

	// Shake stuff
	private float shakeDur = 0f;
	private float shakeIntensity;
	private float shakeDecay;

	/* Instance Methods */
	public void Awake()
	{
		if (scene_cam == null)
			scene_cam = this;
		else
			throw new UnityException ("More than one rig in the scene, dummy");

		cam = transform.GetChild (0);

		path = new Queue<PathNode> ();

		target = null;
		isFollowingTarget = false;
	}

	public void Update()
	{
		//shake the camera
		if (shakeDur > 0f)
		{
			cam.localPosition = (Vector3)Random.insideUnitCircle * shakeIntensity;

			shakeIntensity -= shakeDecay * Time.deltaTime;
			if (shakeIntensity <= 0f)
				shakeDur = 0f;

			shakeDur -= Time.deltaTime;
			if (shakeDur <= 0f)
			{
				shakeDur = 0f;
				shakeIntensity = 0f;
				shakeDecay = 0f;

				cam.localPosition = Vector2.zero;
			}
		}

		//follow a target or transition through a path
		if (isFollowingTarget && target != null)
			transform.position = Vector3.Lerp (transform.position, target.position, Time.deltaTime);
		else if(path.Count > 0)
		{
			StartCoroutine (transitionToPathNode (path.Peek (), 1f));
			StartCoroutine (waitForNode (path.Dequeue ()));
		}
	}

	// Assigns the target and toggles target following
	public void setTarget(Transform target)
	{
		this.target = target;
		isFollowingTarget = true;
	}

	// Unassigns the target and frees the camera
	public void freeCamera()
	{
		target = null;
		isFollowingTarget = false;
	}

	// Shakes the camera for some a duration
	public void shakeCamera(float duration, float intensity, float decayRate = 0f)
	{
		shakeDur = duration;
		shakeIntensity = intensity;
		shakeDecay = decayRate;
	}

	// Adds a PathNode to the path queue
	public void addNodeToPath(PathNode node)
	{
		path.Enqueue (node);
	}
	public void addNodeToPath(Vector2 position, float duration)
	{
		path.Enqueue (new PathNode (position, transform.rotation, duration));
	}
	public void addNodesToPath(params PathNode[] nodes)
	{
		foreach (PathNode node in nodes)
			path.Enqueue (node);
	}

	// Transitions the camera to a PathNode
	private IEnumerator transitionToPathNode(PathNode node, float duration)
	{
		Vector3 nodePos = new Vector3 (node.position.x, node.position.y, transform.position.z);
		Camera c = cam.GetComponent<Camera> ();

		while (duration > 0f)
		{
			transform.position = Vector3.Lerp (transform.position, nodePos, Time.deltaTime);
			transform.rotation = Quaternion.Lerp (transform.rotation, node.rotation, Time.deltaTime);
			if(node.changeZoom)
				c.orthographicSize = Mathf.Lerp (c.orthographicSize, node.zoomLevel, Time.deltaTime);

			duration -= Time.deltaTime;
			yield return null;
		}

		transform.position = nodePos;
		transform.rotation = node.rotation;
		if (node.changeZoom)
			c.orthographicSize = node.zoomLevel;
	}
	private IEnumerator waitForNode(PathNode node)
	{
		yield return new WaitForSeconds (node.duration);
	}

	// Inner struct that is used in controlling camera movements outside of following or shaking
	public struct PathNode
	{
		/* Instance Vars */

		// Where the node is
		public readonly Vector2 position;

		// The rotation of the node
		public readonly Quaternion rotation;

		// How long the node lasts in a chain sequence
		public readonly float duration;

		// The size of the orthographic viewport when on this node
		public readonly float zoomLevel;
		public readonly bool changeZoom;

		/* Constructors */
		public PathNode(Vector2 position, Quaternion rotation, float duration)
		{
			this.position = position;
			this.rotation = rotation;
			this.duration = duration;
			zoomLevel = 0f;
			changeZoom = false;
		}
		public PathNode(Vector2 position, Quaternion rotation, float duration, float zoomLevel) : this(position, rotation, duration)
		{
			this.zoomLevel = zoomLevel;
			changeZoom = true;
		}

		public override bool Equals (object obj)
		{
			PathNode other = (PathNode)obj;
			return (this.position.Equals (other.position)) &&
			(this.rotation.Equals (other.rotation)) &&
			(this.duration == other.duration) &&
			(this.changeZoom == other.changeZoom) &&
			(this.zoomLevel == other.zoomLevel);
		}
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override string ToString ()
		{
			string pos = position.ToString ();
			string rot = rotation.ToString ();
			string dur = duration.ToString ("###.##");
			string zoom = zoomLevel.ToString ("###.##");
			string isZoom = changeZoom.ToString ();

			return "PathNode:\n\tPosition: " + pos + "\n\tRotation: " + rot +
				"\n\tDuration: " + dur + "\n\tZoom: " + zoom + "\n\tChanges Zoom: " + isZoom;
		}
	}
}
