using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private float _raycastDistance;
    [SerializeField] private Camera _raycastCamera;


    [Header("UI")]
    [SerializeField] private RectTransform _interactionIndicator;
    private Tween _scaleTween;
    private bool _looking = true;

    private iInteractable _currentInteractable;
    private iInteractable _currentActiveInteractable;
    private bool _interacting;
    private List<object> _controllerBlockers = new();
    private bool _block = false;
    private Vector3 CenterScreen => new Vector3(Screen.width / 2, Screen.height / 2, 0);

    private void OnEnable()
    {
        PlayerInput.OnInteractActionMain += Interact;
    }

    private void OnDisable()
    {
        PlayerInput.OnInteractActionMain -= Interact;
    }

    public void BlockInteractions(object origin, bool state)
    {
        if (state && !_controllerBlockers.Contains(origin))
            _controllerBlockers.Add(origin);
        else if (!state && _controllerBlockers.Contains(origin))
            _controllerBlockers.Remove(origin);

        _block = _controllerBlockers.Count > 0;
    }


    private void FixedUpdate()
    {
        if (_block)
            return;

        var ray = _raycastCamera.ScreenPointToRay(CenterScreen);
        if (Physics.Raycast(ray, out RaycastHit objectHit, _raycastDistance))
        {
            if (objectHit.transform.TryGetComponent<iInteractable>(out var interactable))
            {
                OnInteractableHit(interactable);
            }
            else
            {
                OnInteractableStopHit();
            }
        }
        else
        {
            OnInteractableStopHit();
        }
    }

    private void Interact(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!context.canceled)
            return;

        if (_interacting)
            return;

        if (_currentInteractable != null)
        {
            _interacting = true;
            _currentInteractable.Interact();
            _currentInteractable.OnInteractionEnter();
        }
    }

    //display visual feedback that you are hitting an interactable
    public void OnInteractableHit(iInteractable interactableHit)
    {
        _currentInteractable = interactableHit;
        _currentInteractable.InteractableHit(this);


        if (!_looking)
        {
            _looking = true;
            _scaleTween?.Kill();
            _scaleTween = _interactionIndicator.DOScale(1, .2f);
            _scaleTween.OnComplete(() => _scaleTween = null);
        }
    }

    //stop displaying feedback that you are hitting an interactable
    public void OnInteractableStopHit()
    {
        _currentInteractable = null;

        if (_looking)
        {
            _looking = false;
            _scaleTween?.Kill();
            _scaleTween = _interactionIndicator.DOScale(0, .2f);
            _scaleTween.OnComplete(() => _scaleTween = null);
        }
    }

    public void Interact()
    {

    }

    public void ExitInteract()
    {
        if (_currentInteractable != null)
            _currentInteractable.OnInteractionExit();
        _currentInteractable = null;
        _interacting = false;
    }

}
