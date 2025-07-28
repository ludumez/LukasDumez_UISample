using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSelectableElement : SelectableElement
{
    //Displays a target skill and handles the unlocking of it
    //Display the needed resources to unlock the skill
    //Animates the unlocking of the skill

    [SerializeField] private Image _skillIconImage;
    [SerializeField] private TMP_Text _resourceRequiredText;
    [SerializeField] private RectTransform _resourceNeededHolder;
    [SerializeField] private Image _unlockProgressionFillImage;
    [SerializeField] private Skills _targetSkill;
    [SerializeField] private Image _lockedOverlayImage;
    [SerializeField] private Image _buttonImage;

    private SkillSetup _targetSkillSetup;
    private SkillInventory _skillInventory;
    private InventoryManager _inventoryManager;
    private SkillManager _skillManager;
    private bool _isUnlocked;

    private Tween _scaleTween;
    private Tween _fillTween;

    protected override void OnEnable()
    {
        base.OnEnable();
        SkillManager.OnSkillUnlocked += UpdateSkill;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SkillManager.OnSkillUnlocked -= UpdateSkill;

        _scaleTween?.Kill();
        _fillTween?.Kill();
    }


    public void Init(InventoryManager inventoryManager, SkillManager skillManager, SkillInventory skillInventory)
    {
        _inventoryManager = inventoryManager;
        _skillManager = skillManager;
        _skillInventory = skillInventory;

        _targetSkillSetup = _skillManager.GetSkillSetup(_targetSkill);
        _currentElement = _targetSkill;

        ResetElement();
    }

    //Update element if the current element has been unlocked
    private void UpdateSkill(SkillSetup currentUpdatedSkill)
    {
        SetupElement();
    }


    public override void SetupElement()
    {
        base.SetupElement();
        _skillIconImage.sprite = _currentElement.ElementSprite;
        _skillIconImage.enabled = _currentElement.ElementSprite != null;

        bool canInteract = _skillManager.HasSkillRequiremenet(_targetSkill);
        interactable = canInteract;
        _buttonImage.enabled = canInteract;

        if (_targetSkillSetup.UnlockStatus)
        {
            _lockedOverlayImage.enabled = false;
            _resourceNeededHolder.DOScale(0, .2f).OnComplete(() => _resourceNeededHolder.gameObject.SetActive(false));
        }
        else if (canInteract)
        {
            _lockedOverlayImage.transform.DOScale(0, .2f).OnComplete(() => _lockedOverlayImage.enabled = false);
            _resourceNeededHolder.gameObject.SetActive(true);
            _resourceNeededHolder.DOScale(1, .2f);
            ConstructText();
            transform.SetAsLastSibling();//set the order of the transform to first to avoid overlaping of the resource needed holder
        }
        else
        {
            _lockedOverlayImage.enabled = true;
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        //_resourceNeededHolder.localScale = Vector3.zero;
        //_resourceNeededHolder.gameObject.SetActive(true);
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(1.1f, .2f);

        _elementInspector.DisplayElement(_currentElement, true);
        transform.SetAsLastSibling();//set the order of the transform to first to avoid overlaping of the resource needed holder
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);

        //Only trigger unlock sequence if element is not unlocked
        if (_skillInventory.CanUnlockSkill(_targetSkillSetup))
            StartCoroutine(Sequence());

        IEnumerator Sequence()
        {
            _fillTween?.Kill();
            _unlockProgressionFillImage.fillAmount = 1;
            _unlockProgressionFillImage.color = new Color(0, 0, 0, 0);
            transform.DOPunchScale(Vector3.one * .2f, .2f);

            //_unlockProgressionFillImage.DOColor(new Color(1, 1, 1, 1), .1f);
            //yield return new WaitForSeconds(.1f);
            _unlockProgressionFillImage.DOColor(new Color(0, 0, 0, 1), .1f);
            yield return new WaitForSeconds(.1f);
            _fillTween = _unlockProgressionFillImage.DOFillAmount(0, .2f);

            _skillInventory.UnlockSkill(_targetSkillSetup);
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(1, .2f);

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        OnSelect(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        OnDeselect(eventData);
    }

    private void ResetElement()
    {
        _lockedOverlayImage.transform.localScale = Vector3.one;
        _unlockProgressionFillImage.fillAmount = 0;
        _resourceNeededHolder.transform.localScale = Vector3.zero;
        _resourceNeededHolder.gameObject.SetActive(false);
    }

    private void ConstructText()
    {
        int fishRequiredAmount = _skillInventory.GetNeededFishReource(_targetSkillSetup);
        int exploreRequiredAmount = _skillInventory.GetNeededExplorationResource(_targetSkillSetup);

        int fishAvailableAmount = _skillInventory.AvaialableFishResource();
        int exploreAvailableAmount = _skillInventory.AvailableExplorationResoruce();

        string text = string.Empty;

        if (fishRequiredAmount != 0)
        {
            if (text != string.Empty)
                text += "\n";//Separator if both resources are needed

            text += $"{fishAvailableAmount}/{fishRequiredAmount} <sprite index=2>";
        }

        if (exploreRequiredAmount != 0)
        {

            if (text != string.Empty)
                text += "\n";//Separator if both resources are needed

            text += $"{exploreAvailableAmount}/{exploreRequiredAmount} <sprite index=1>";
        }
        _resourceRequiredText.SetText(text);
        _resourceNeededHolder.gameObject.SetActive(text != string.Empty);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        OnSubmit(eventData);
    }
}
