using DG.Tweening;
using UnityEngine;

public class FishingSpot : MonoBehaviour, iInteractable
{
    private InteractionManager _interactionManager;

    //Reference set manually for now
    [SerializeField] private FishingManager _fishingManager;

    public void Interact()
    {
        _fishingManager.EnterFishing(this);
    }

    public void InteractableHit(InteractionManager manager)
    {
        _interactionManager = manager;
    }

    public void OnInteractionEnter()
    {
        transform.DOScale(0, .2f);
    }

    public void OnInteractionExit()
    {
        transform.DOScale(1, .2f);
    }

    public void StopInteract()
    {
        _interactionManager.ExitInteract();
    }
    public void FishingCollected()
    {
        gameObject.SetActive(false);
    }
}
