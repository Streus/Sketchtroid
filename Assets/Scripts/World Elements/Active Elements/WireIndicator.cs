using UnityEngine;
using CircuitNodes;

[RequireComponent(typeof(SpriteRenderer))]
public class WireIndicator : LeafNode
{
	[SerializeField]
	private Color activeColor, inactiveColor;

	private SpriteRenderer rend;

	public void Awake()
	{
		rend = GetComponent<SpriteRenderer> ();
	}

	public void Update()
	{
		if (isActivated ())
			rend.color = activeColor;
		else
			rend.color = inactiveColor;
	}
}
