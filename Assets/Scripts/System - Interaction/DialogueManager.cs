using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    #region Singleton
    public static DialogueManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void OnDestroy()
    {
        Destroy(gameObject);
    }
    #endregion


    [Header("References")]
    [SerializeField] private TMP_Text _dialogueTextDisplay;
    [SerializeField] private RectTransform _dialogueHolder;

    [Header("Managers")]
    [SerializeField] private PlayerStateManager _playerStateManager;
    [SerializeField] private CameraController _cameraController;

    [Header("Test")]
    [SerializeField] private Button _closeDialogue;

    private Conversant _currentConversant;

    public void DisplayDialogue(Conversant origin)
    {
        _currentConversant = origin;

        LoadOptions();
        LoadText();
        _dialogueHolder.gameObject.SetActive(true);

        _playerStateManager.SelectPlayerState(PlayerState.InUI);
        _cameraController.FocusOnTarget(_currentConversant.gameObject);
    }

    private void LoadOptions()
    {
        _closeDialogue.onClick.RemoveAllListeners();
        _closeDialogue.onClick.AddListener(() => CloseDialogue());
    }

    private void LoadText()
    {
        _dialogueTextDisplay.text = _currentConversant.TargetDialogue.DialogueText;
    }

    private void CloseDialogue()
    {
        _dialogueHolder.gameObject.SetActive(false);
        _playerStateManager.SelectPlayerState(PlayerState.WalkingAround);
        _cameraController.RevertFocus();
        _currentConversant.StopInteract();
    }
}
