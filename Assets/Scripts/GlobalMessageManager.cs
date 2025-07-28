using System.Collections;
using TMPro;
using UnityEngine;

public class GlobalMessageManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _targetText;
    [SerializeField] private float _typeSpeed;
    [SerializeField] private float _lifeTimePerCharacter;

    private void Start()
    {
        _targetText.enabled = false;
    }

    public void DisplayText(string text)
    {
        StartCoroutine(Sequence());
        IEnumerator Sequence()
        {
            _targetText.enabled = true;
            _targetText.maxVisibleCharacters = 0;
            while (_targetText.maxVisibleCharacters < text.Length)
            {
                _targetText.maxVisibleCharacters++;
                yield return new WaitForSeconds(_typeSpeed);
            }
            yield return new WaitForSeconds(_lifeTimePerCharacter * text.Length);
            _targetText.enabled = false;
        }
    }
}
