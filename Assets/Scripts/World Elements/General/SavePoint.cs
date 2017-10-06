using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
	[SerializeField]
	private float saveCooldownMax = 2f;
	private float saveCooldown = 0f;
	[SerializeField]
	private float rotationSpeed = 1.2f;
	[SerializeField]
	private Color pulseColor;
	[SerializeField]
	private float pulseRate = 2f;
	[SerializeField]
	private bool compressed = false;

	[SerializeField]
	private SpriteRenderer baseImage;
	private Vector2 extendedSize;
	[SerializeField]
	private Vector2 compressedSize;

	private bool lockingPlayer = false;
	private Player player;

	public void Awake()
	{
		if (baseImage == null)
			throw new UnassignedReferenceException ("Forgot to set the base image, ya dummy!");

		saveCooldown = saveCooldownMax * 3;

		extendedSize = baseImage.size;
	}

	public void Update()
	{
		saveCooldown -= Time.deltaTime;

		if (compressed || saveCooldown > 0f)
		{
			baseImage.size = Vector2.Lerp (baseImage.size, compressedSize, Time.deltaTime * 3f);
		}
		else
		{
			transform.Rotate (0f, 0f, rotationSpeed);
			baseImage.size = Vector2.Lerp (baseImage.size, extendedSize, Time.deltaTime);
			baseImage.color = Color.Lerp (Color.black, pulseColor, Mathf.Sin (Time.time * pulseRate));
		}

		if (lockingPlayer)
		{
			player.transform.position = Vector3.Lerp (player.transform.position, transform.position, Time.deltaTime);
			player.transform.Rotate (0f, 0f, rotationSpeed);
			if (Vector3.Distance (player.transform.position, transform.position) < 0.1f)
			{
				//TODO add special effect for save operation ?
				GameManager.instance.saveGame ();
				lockingPlayer = false;
				player.enabled = true;
				saveCooldown = saveCooldownMax;
				player = null;
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		Player temp = col.GetComponent<Player> ();
		if (temp != null && saveCooldown <= 0f)
		{
			player = temp;
			compressed = true;
			player.enabled = false;
			lockingPlayer = true;
		}
	}

	public void OnTriggerExit2D(Collider2D col)
	{
		if(col.GetComponent<Player>() != null)
			compressed = false;
	}
}
