using System.Collections.Generic;
using UnityEngine;

public class StepManager : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _audioClips = new List<AudioClip>();
    [SerializeField] private float _stepLength;

    public void OnStep()
    {
        _audioSource.clip = _audioClips[Random.Range(0, _audioClips.Count)];
        _audioSource.Play();
    }
}
