using System.Collections;
using UnityEngine;

public class BoomBox : MonoBehaviour, iInteractable
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _targetSoundClip;
    [SerializeField] private AudioClip _toggleBoomBoxSFX;
    [SerializeField] private Animator _animator;

    private Coroutine _coroutine;
    private InteractionManager _interactionManager;
    private bool _isPlaying;
    private const string AnimationClip = "BoomBoxOnEnable";
    public void Interact()
    {
        _isPlaying = !_isPlaying;


        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(Sequence());
        IEnumerator Sequence()
        {
            _audioSource.clip = _toggleBoomBoxSFX;
            _audioSource.Play();
            _animator.enabled = true;
            _animator.Play(AnimationClip);
            yield return new WaitForSeconds(_toggleBoomBoxSFX.length);

            if (_isPlaying)
            {
                _audioSource.clip = _targetSoundClip;
                _audioSource.Play();
            }
            yield return new WaitForEndOfFrame();
            _interactionManager.ExitInteract();
        }
    }

    public void StopInteract()
    {
    }

    public void OnInteractionEnter()
    {
    }

    public void OnInteractionExit()
    {
    }

    public void InteractableHit(InteractionManager manager)
    {
        _interactionManager = manager;
    }

    private void OnTriggerExit(Collider other)
    {
        _isPlaying = false;
        _audioSource.Stop();
    }
}
