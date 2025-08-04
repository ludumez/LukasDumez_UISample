using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillInventory : Menu
{
    [SerializeField] private SkillSelectableElement _defaultSelectableElement;
    [SerializeField] private SelectableElementInspector _selectableElementInspector;

    [Header("Currency settings")]
    [SerializeField] private RectTransform _sideBody1;
    [SerializeField] private RectTransform _sideBody2;
    [SerializeField] private TMP_Text _fishResourceText;
    [SerializeField] private TMP_Text _explorationResourceText;
    [SerializeField] private Item _explorationResource;
    [SerializeField] private Item _fishResource;
    [SerializeField] private AnimationCurve _resourceTransitionCurve;

    [Header("Exit Indication References")]
    [SerializeField] private Image _exitImage;
    [SerializeField] private InventoryExitButton _exitButton;

    private SkillSelectableElement[] _selectableSkillElements;
    private InventoryManager _inventoryManager;
    private SkillManager _skillManager;
    private Tween _sideBody1Tween;
    private Tween _sideBody2Tween;

    private Coroutine _fillSequence;
    private Coroutine _emptySequence;
    private Coroutine _changeMenuSequence;
    private bool _stopInteraction;//Disable any animation from input 
    private Tween _scaleTween;

    public override void InitMenu(EventSystem eventSystem, MenuController menuController)
    {
        base.InitMenu(eventSystem, menuController);

        _inventoryManager = InventoryManager.Instance;
        _skillManager = SkillManager.Instance;

        _selectableSkillElements = transform.GetComponentsInChildren<SkillSelectableElement>(true);
        foreach (var selectableElement in _selectableSkillElements)
        {
            selectableElement.Init(_selectableElementInspector);
            selectableElement.Init(_inventoryManager, _skillManager, this);
            selectableElement.SetupElement();
        }

        _eventSystem.SetSelectedGameObject(_defaultSelectableElement.gameObject);
        _exitButton.Init(_menuController);
    }


    private void OnEnable()
    {
        SkillManager.OnSkillUnlocked += OnSKillsUpdates;
    }

    private void OnDisable()
    {
        SkillManager.OnSkillUnlocked -= OnSKillsUpdates;

        _sideBody1Tween?.Kill();
        _sideBody2Tween?.Kill();
    }



    public override void ForceSelection()
    {
        base.ForceSelection();
        _eventSystem.SetSelectedGameObject(_defaultSelectableElement.gameObject);
    }

    public override void MenuOpenFinished()
    {
        base.MenuOpenFinished();
        ForceSelection();

        _sideBody1.gameObject.SetActive(true);
        _sideBody1Tween?.Kill();
        _sideBody1Tween = _sideBody1.DOAnchorPosX(0, .3f).SetEase(_resourceTransitionCurve).SetDelay(.05f);

        _sideBody2.gameObject.SetActive(true);
        _sideBody2Tween?.Kill();
        _sideBody2Tween = _sideBody2.DOAnchorPosX(0, .3f).SetEase(_resourceTransitionCurve).SetDelay(.15f);
    }


    public override void MenuOpen()
    {
        base.MenuOpen();
        gameObject.SetActive(true);

        _exitImage.fillAmount = 0;
        _stopInteraction = false;

        _sideBody1.gameObject.SetActive(false);
        _sideBody1.anchoredPosition = new Vector2(_sideBody1.sizeDelta.x, _sideBody1.anchoredPosition.y);

        _sideBody2.gameObject.SetActive(false);
        _sideBody2.anchoredPosition = new Vector2(_sideBody2.sizeDelta.x, _sideBody2.anchoredPosition.y);

        UpdateCurrencyDisplay();

        foreach (var selectableElement in _selectableSkillElements)
        {
            selectableElement.SetupElement();
        }
    }

    public override void MenuClose()
    {
        base.MenuClose();
        gameObject.SetActive(false);
    }

    private void OnSKillsUpdates(SkillSetup setup)
    {
        UpdateCurrencyDisplay();
    }

    private void UpdateCurrencyDisplay()
    {
        _explorationResourceText.SetText(_inventoryManager.GetItemAmount(_explorationResource).ToString() + " <sprite index=2> ");
        _fishResourceText.SetText(_inventoryManager.GetItemAmount(_fishResource).ToString() + " <sprite index=1> ");
    }

    public int GetNeededFishResource(SkillSetup skillSetup)
    {
        foreach (var item in skillSetup.RequiredItems)
        {
            if (item.Item == _fishResource)
                return item.Amount;
        }

        return 0;
    }

    public int GetNeededExplorationResource(SkillSetup skillSetup)
    {
        foreach (var item in skillSetup.RequiredItems)
        {
            if (item.Item == _explorationResource)
                return item.Amount;
        }

        return 0;
    }

    public int AvailableExplorationResource()
    {
        return _inventoryManager.GetItemAmount(_explorationResource);
    }

    public int AvailableFishResource()
    {
        return _inventoryManager.GetItemAmount(_fishResource);
    }

    public bool CanUnlockSkill(SkillSetup targetSkill)
    {
        //if it is already unlock do not unlock again
        if (targetSkill.UnlockStatus)
            return false;

        foreach (var requiredItem in targetSkill.RequiredItems)
        {
            if (requiredItem.Item is not Skills)
                if (_inventoryManager.GetItemAmount(requiredItem.Item) < requiredItem.Amount)
                    return false;
        }
        return true;
    }

    public void UnlockSkill(SkillSetup targetSkill)
    {
        foreach (var requiredItem in targetSkill.RequiredItems)
        {
            if (requiredItem.Item is Skills)
                continue;

            _inventoryManager.RemoveItem(requiredItem.Item, requiredItem.Amount);
        }
        _skillManager.UnlockSkill(targetSkill.Skill);
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
