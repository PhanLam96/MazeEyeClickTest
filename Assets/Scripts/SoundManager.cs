using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> clipList;

    public void PlaySound(int clipIndex)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = clipList[clipIndex];
            audioSource.Play();
        }
    }
    public void StopSound() => audioSource.Stop();
}
