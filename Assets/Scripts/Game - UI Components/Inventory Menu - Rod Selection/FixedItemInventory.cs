using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FixedItemInventory : Menu, iInventoryInterface
{
    //Inventory to hold all fishing rods
    //Doesn't allow any empty inventory slots
    //All items are generated in a horizontal row


    public InventoryManager InventoryManager { get => _inventoryManager; }
    public MenuController MenuController { get => _menuController; }
    private HookSelectionManager _hookSelectionManager;
    private InventoryManager _inventoryManager;

    [Header("Inventory")]
    [SerializeField] private SelectableElementAlwaysFull _fishingRodInventoryElementPrefab;
    [SerializeField] private RectTransform _elementsHolder;
    [SerializeField] private Scrollbar _horizontalScrollbar;
    [SerializeField] private float _automaticScroolSpeed = 0.5f;
    [SerializeField] private AnimationCurve _scrollAnimationCurve;
    [SerializeField] private AnimationCurve _iconBounceAnimationCurve;
    private List<SelectableElementAlwaysFull> _currentElementInstances = new List<SelectableElementAlwaysFull>();
    private Coroutine _sliderMovementRoutine;

    [Header("Exit Indications")]
    [SerializeField] private InventoryExitButton _exitButton;
    [SerializeField] private Image _exitImage;
    private bool _stopInteraction = false;
    private Coroutine _emptySequence;
    private Coroutine _fillSequence;
    private Coroutine _changeMenuSequence;
    private Tween _scaleTween;


    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void InitInventory(InventoryManager inventoryManager)
    {
        _inventoryManager = inventoryManager;
    }

    public override void MenuOpen()
    {
        base.MenuOpen();
        gameObject.SetActive(true);
        _hookSelectionManager = HookSelectionManager.Instance;
        GenerateInventory();
        GenerateNavigation();

        _exitButton.Init(_menuController);

        //we delay an initial selection for one frame to allow the ui to be rebuilt properly 
        //rebuild by the unity layout builder 
        StartCoroutine(DelayedSelection());
        IEnumerator DelayedSelection()
        {
            yield return new WaitForEndOfFrame();
            _eventSystem.SetSelectedGameObject(_currentElementInstances[0].gameObject);
        }
    }

    public override void MenuClose()
    {
        base.MenuClose();
        gameObject.SetActive(false);
    }

    public override void ForceSelection()
    {
        base.ForceSelection();
        if (_currentElementInstances.Count > 0)
            _eventSystem.SetSelectedGameObject(_currentElementInstances[_currentElementInstances.Count - 1].gameObject);
    }

    private void GenerateInventory()
    {
        //Clear previous elements
        foreach (Transform element in _elementsHolder)
        {
            Destroy(element.gameObject);
        }
        _currentElementInstances.Clear();
        foreach (var item in _inventoryManager.GetItemsOfType<ItemHook>())
        {
            SelectableElementAlwaysFull element = Instantiate(_fishingRodInventoryElementPrefab, _elementsHolder);
            element.InitItem(item.Key as iDisplayeableElement);
            element.SetupElement();
            _currentElementInstances.Add(element);
            element.Init(() => OnElementSelected(element), _hookSelectionManager, _menuController);
        }
    }

    private void GenerateNavigation()
    {
        for (int i = 0; i < _currentElementInstances.Count; i++)
        {
            Selectable selectable = _currentElementInstances[i];
            var nav = selectable.navigation;
            nav.mode = Navigation.Mode.Explicit;

            int count = _currentElementInstances.Count;
            Selectable leftSelectableElement = _currentElementInstances[(count + i - 1) % count];
            if (leftSelectableElement.interactable)
                nav.selectOnLeft = leftSelectableElement;
            Selectable rightSelectableElement = _currentElementInstances[(i + 1) % count];
            if (rightSelectableElement.interactable)
                nav.selectOnRight = rightSelectableElement;

            selectable.navigation = nav;
        }
    }

    private void OnElementSelected(SelectableElementAlwaysFull element)
    {
        float proportionalX = element.transform.GetSiblingIndex() / ((float)_currentElementInstances.Count - 1);

        if (_sliderMovementRoutine != null)
            StopCoroutine(_sliderMovementRoutine);

        _sliderMovementRoutine = StartCoroutine(SliderSequence(proportionalX));
        IEnumerator SliderSequence(float targetValue)
        {
            float targetSliderValue = targetValue;
            //time bound and not speed bound to allow control by animation curve
            //time bound allows for easier end check to properly cleanup coroutine
            //speed = distance/time  => time = distance/speed
            float duration = (Mathf.Abs(_horizontalScrollbar.value - targetValue)) / _automaticScroolSpeed;
            float t = 0;
            while (t < duration)
            {
                yield return new WaitForFixedUpdate();

                _horizontalScrollbar.onValueChanged.Invoke(Mathf.Lerp(_horizontalScrollbar.value, targetSliderValue, _scrollAnimationCurve.Evaluate(t / duration)));
                t += Time.fixedDeltaTime;
            }
            _sliderMovementRoutine = null;
        }
    }

    public override void LeaveMenu(InputAction.CallbackContext context)
    {
        base.LeaveMenu(context);
        if (_stopInteraction)
            return;

        if (context.started)
        {
            if (_emptySequence != null)
                StopCoroutine(_emptySequence);
            if (_fillSequence != null)
                StopCoroutine(_fillSequence);
            _fillSequence = StartCoroutine(FillSequence());
        }
        else if (context.canceled)
        {
            if (_emptySequence != null)
                StopCoroutine(_emptySequence);
            if (_fillSequence != null)
                StopCoroutine(_fillSequence);
            _emptySequence = StartCoroutine(EmptySequence());
        }

        IEnumerator FillSequence()
        {
            while (_exitImage.fillAmount < 1)
            {
                yield return new WaitForFixedUpdate();
                _exitImage.fillAmount += _menuController.ExitInputDuration * Time.fixedDeltaTime;
            }

            if (_changeMenuSequence != null)
                StopCoroutine(_changeMenuSequence);
            _changeMenuSequence = StartCoroutine(ChangeMenuSequence());
            _fillSequence = null;
        }

        IEnumerator EmptySequence()
        {
            while (_exitImage.fillAmount > 0)
            {
                yield return new WaitForFixedUpdate();
                _exitImage.fillAmount -= _menuController.ExitInputDuration * Time.fixedDeltaTime;
            }
            _emptySequence = null;
        }

        IEnumerator ChangeMenuSequence()
        {
            _stopInteraction = true;

            _scaleTween?.Kill();
            _scaleTween = _exitImage.transform.DOPunchScale(Vector3.one * .2f, .2f);
            yield return new WaitForSeconds(.3f);
            _menuController.OnMenuLeft();
        }
    }

}
