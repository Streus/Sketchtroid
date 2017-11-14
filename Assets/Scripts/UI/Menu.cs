using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(CanvasGroup))]
public class Menu : MonoBehaviour
{
	private Animator animator;
	private CanvasGroup canvasGroup;

	public bool IsOpen
	{
		get{ return animator.GetBool("IsOpen"); }
		set
		{
			animator.SetBool("IsOpen", value);
			canvasGroup.blocksRaycasts = canvasGroup.interactable = value;
			OnFocusChanged (value);
		}
	}

	public void Awake()
	{
		animator = GetComponent<Animator>();
		canvasGroup = GetComponent<CanvasGroup>();

		//move this menu onto the canvas
		RectTransform rect = GetComponent<RectTransform>();
		rect.offsetMax = rect.offsetMin = Vector2.zero;
	}

	public delegate void FocusChanged(bool inFocus);
	public event FocusChanged changedFocus;
	public void OnFocusChanged(bool inFocus)
	{
		if (changedFocus != null)
			changedFocus (inFocus);
	}
}
