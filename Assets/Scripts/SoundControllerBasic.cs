using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControllerBasic : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundClip;
    private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    //反転時の効果音
    public void PlayHantenSound()
    {
        audioSource.PlayOneShot(soundClip[0]);
    }

    //ダッシュタメの効果音
    public void PlayDashTameSound()
    {
        audioSource.PlayOneShot(soundClip[1]);
    }


    //ダッシュの効果音
    public void PlayDashSound()
    {
        audioSource.PlayOneShot(soundClip[2]);
    }



}
