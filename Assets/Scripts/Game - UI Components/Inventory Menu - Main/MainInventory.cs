using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainInventory : Menu
{
    //Main inventory that displays teh current selected bait and rod
    //Provides access to the skill, bait and rod menu
    //Display the current hovered item information

    [SerializeField] private Button _defaultButtonSelection;
    [SerializeField] private SelectableElementInspector _selectableElementInspector;

    [Header("Navigation Element")]
    [SerializeField] private BaitSelectionSelectableElementMenuTrigger _baitSelectionElement;
    [SerializeField] private SkillTreeSelectableElementMenuTrigger _skillTreeSelectableElement;
    [SerializeField] private RodSelectionElementMenuTrigger _rodSelectableElement;

    [Header("Menus")]
    [SerializeField] private MaxSizeInventory _maxSizeInventory;
    [SerializeField] private SkillInventory _skillInventory;
    [SerializeField] private FixedItemInventory _fixedItemInventory;

    private BaitSelectionManager _baitSelectionManager;
    private HookSelectionManager _hookSelectionManager;

    private void OnEnable()
    {
        ReflectCurrentInput.OnInputTypeChanged += OnInputDeviceChanged;
    }
    private void OnDisable()
    {
        ReflectCurrentInput.OnInputTypeChanged -= OnInputDeviceChanged;
    }

    private void OnInputDeviceChanged(InputType type)
    {
        //if we a using a mouse we want deselect the current selected object
        if (type == InputType.Mouse)
            _eventSystem.SetSelectedGameObject(null);
    }

    public override void InitMenu(EventSystem eventSystem, MenuController menuController)
    {
        base.InitMenu(eventSystem, menuController);

        _baitSelectionManager = BaitSelectionManager.Instance;
        _hookSelectionManager = HookSelectionManager.Instance;

        _baitSelectionElement.Init(_menuController, _baitSelectionManager, _maxSizeInventory);
        _skillTreeSelectableElement.Init(menuController, _skillInventory);
        _rodSelectableElement.Init(menuController, _fixedItemInventory, _hookSelectionManager);

        _selectableElementInspector.DisplayElement(null, false);
    }

    public override void MenuOpen()
    {
        gameObject.SetActive(true);

        _baitSelectionElement.Init(_selectableElementInspector);
        _baitSelectionElement.InitItem(_baitSelectionManager.CurrentSelectedBait);
        _rodSelectableElement.Init(_selectableElementInspector);
        _rodSelectableElement.InitItem(_hookSelectionManager.CurrentHookItem);

        _selectableElementInspector.DisplayElement(null, false);
    }

    public override void MenuClose()
    {
        gameObject.SetActive(false);
    }

    public override void ForceSelection()
    {
        base.ForceSelection();
        _eventSystem.SetSelectedGameObject(_defaultButtonSelection.gameObject);
    }


    public override void MenuOpenFinished()
    {
        base.MenuOpenFinished();
        _eventSystem.SetSelectedGameObject(_defaultButtonSelection.gameObject);
    }
}
