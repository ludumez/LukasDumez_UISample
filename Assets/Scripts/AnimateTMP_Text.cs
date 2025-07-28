using System.Collections;
using TMPro;
using UnityEngine;

public static class AnimateTMP_Text
{
    public static void Animate(this TMP_Text targetText, float timeBetweenChar = .05f, bool isTimeScaleDependent = true)
    {
        targetText.StopAllCoroutines();
        targetText.StartCoroutine(AnimateText(isTimeScaleDependent));
        IEnumerator AnimateText(bool timeScaleDependent)
        {
            targetText.maxVisibleCharacters = 0;
            while (targetText.maxVisibleCharacters < targetText.text.Length)
            {
                if (!timeScaleDependent)
                    yield return new WaitForSecondsRealtime(timeBetweenChar);
                else
                    yield return new WaitForSeconds(timeBetweenChar);

                targetText.maxVisibleCharacters++;
            }
        }
    }

    public static void StopAnimation(this TMP_Text targetText)
    {
        targetText.StopAllCoroutines();
        targetText.maxVisibleCharacters = 0;
    }
}
