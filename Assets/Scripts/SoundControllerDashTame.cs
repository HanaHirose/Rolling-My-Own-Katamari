using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControllerDashTame : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundClip;
    private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    //このオーディオソースの音をすべて止める
    public void StopSound()
    {
        audioSource.Stop();
    }

    //ダッシュタメの効果音
    public void PlayDashTameSound()
    {
        audioSource.PlayOneShot(soundClip[0]);
    }
    public void StopDashTameSound()
    {
        audioSource.Stop();
    }


    //ブレーキの音
    public void PlayBrakeSoundShort()
    {
        audioSource.PlayOneShot(soundClip[1]);
        StartCoroutine(WaitShort());
    }
    public void PlayBrakeSoundMideum()
    {
        audioSource.PlayOneShot(soundClip[1]);
        StartCoroutine(WaitMedium());
    }

    private IEnumerator WaitShort()
    {
        yield return new WaitForSeconds(0.3f);
        audioSource.Stop();
    }

    private IEnumerator WaitMedium()
    {
        yield return new WaitForSeconds(0.6f);
        audioSource.Stop();
    }


    //バウンドの音
    public void PlayBounceSound()
    {
        audioSource.PlayOneShot(soundClip[2]);
    }

    //ぶつかる音A
    public void PlayButsukaruA()
    {
        audioSource.PlayOneShot(soundClip[3]);
    }
}
