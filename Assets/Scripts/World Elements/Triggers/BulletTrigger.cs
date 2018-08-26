using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[ExecuteInEditMode]
public class BulletTrigger : CircuitNodes.Toggle
{
	[SerializeField]
	private DamageType key;

	public void Awake()
	{
		SetColor ();
	}

	#if UNITY_EDITOR
	public void Update()
	{
		if(!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
			SetColor ();
	}
	#endif

	public void OnCollisionEnter2D(Collision2D col)
	{
		Bullet b = col.gameObject.GetComponent<Bullet> ();
		if (b != null && b.DamageType == key)
		{
			SetActive (!IsActivated ());

			SetColor ();
		}
	}

	private void SetColor()
	{
		SpriteRenderer rend = GetComponent<SpriteRenderer> ();
		if (rend != null)
			rend.color = IsActivated () ? Bullet.DamageTypeToColor (key) : Color.black;
	}
}
