using UnityEngine;

public class Conversant : MonoBehaviour, iInteractable
{
    [SerializeField] private DialogueScriptableObject _targetDialogue;
    public DialogueScriptableObject TargetDialogue => _targetDialogue;
    private InteractionManager interactionManager;
    private DialogueManager _dialogueManager;

    private void Start()
    {
        _dialogueManager = DialogueManager.Instance;
    }

    public void Interact()
    {
        _dialogueManager.DisplayDialogue(this);
        interactionManager.Interact();
    }

    public void OnInteractionEnter()
    {

    }

    public void OnInteractionExit()
    {
    }

    public void StopInteract()
    {
        interactionManager.ExitInteract();
    }

    public void InteractableHit(InteractionManager origin)
    {
        interactionManager = origin;
    }
}
