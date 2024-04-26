using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundClip;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomMakikomiSound()
    {
        int randomIndex = Random.Range(0, soundClip.Length);
        audioSource.PlayOneShot(soundClip[randomIndex]);
    }
}
