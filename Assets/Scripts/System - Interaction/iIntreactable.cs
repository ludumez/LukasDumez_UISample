public interface iInteractable
{
    public void Interact();
    public void StopInteract();
    public void OnInteractionEnter();
    public void OnInteractionExit();
    public void InteractableHit(InteractionManager manager);
}
