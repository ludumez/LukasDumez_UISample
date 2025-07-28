using System.Collections;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    [SerializeField] private PlayerStateManager _stateManager;

    [SerializeField] private Animator _rodAnimator;
    private const string LowBiteKey = "LowBite";
    private const string HighBiteKey = "HighBite";
    private const string HighBiteLoopBool = "BiteLoop";

    [Header("Fishing settings")]
    [SerializeField] private int _biteAmount;
    [SerializeField] private Vector2 _timeRangeToBite;
    [SerializeField] private Vector2 _cooldownBetweenBite;
    [SerializeField] private float _reelWaitTime;

    [Header("Collection")]
    [SerializeField] private GlobalMessageManager _messageManager;

    private bool _waitingForReel;
    private Coroutine _fishingRoutine;
    private FishingSpot _currentFishingSpot;

    private void OnEnable()
    {
        PlayerInput.OnInteractActionMain += OnInput;
    }
    private void OnDisable()
    {
        PlayerInput.OnInteractActionMain -= OnInput;
    }


    private void Start()
    {
        _rodAnimator.enabled = false;
        _rodAnimator.gameObject.SetActive(false);
    }

    public void EnterFishing(FishingSpot fishingSpot)
    {
        _currentFishingSpot = fishingSpot;
        _stateManager.SelectPlayerState(PlayerState.Fishing);
        _rodAnimator.enabled = true;
        _rodAnimator.gameObject.SetActive(true);

        _fishingRoutine = StartCoroutine(Sequence());
        IEnumerator Sequence()
        {
            yield return new WaitForSeconds(Random.Range(_timeRangeToBite.x, _timeRangeToBite.y));
            for (int i = 0; i < _biteAmount; i++)
            {
                _rodAnimator.SetTrigger(LowBiteKey);
                yield return new WaitForSeconds(Random.Range(_cooldownBetweenBite.x, _cooldownBetweenBite.y));
                //bite
            }
            _rodAnimator.SetTrigger(HighBiteKey);
            _rodAnimator.SetBool(HighBiteLoopBool, true);

            _waitingForReel = true;
            yield return new WaitForSeconds(_reelWaitTime);
            _waitingForReel = false;

            _rodAnimator.SetBool(HighBiteLoopBool, true);
            OnFishingFailure();
        }
    }


    private void OnInput(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_currentFishingSpot != null && context.canceled && _waitingForReel)
        {
            OnFishingSuccess();
        }
    }


    private void OnFishingSuccess()
    {
        _currentFishingSpot.FishingCollected();
        ResetFishing();
        _messageManager.DisplayText("A memory has been collected");
    }

    private void OnFishingFailure()
    {
        ResetFishing();
    }

    private void ResetFishing()
    {
        _currentFishingSpot.StopInteract();

        _rodAnimator.enabled = false;
        _rodAnimator.gameObject.SetActive(false);
        StopCoroutine(_fishingRoutine);
        _fishingRoutine = null;
        _stateManager.SelectPlayerState(PlayerState.WalkingAround);
        _currentFishingSpot = null;
    }
}
