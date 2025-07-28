using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    #region singleton
    public static CameraShake Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    #endregion


    Vector3 startPosition;

    private void OnEnable()
    {
        startPosition = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalPosition = startPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x, y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }
}
