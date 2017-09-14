
public interface IInteractable
{
	bool interactable{ get; set; }
	bool activated{ get; set; }
	bool bulletActivated{ get; set; }

	DamageType getKeyType();
	void OnInteract();
}