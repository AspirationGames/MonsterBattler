using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] float fadeTime;

    public static AudioManager i;

    float musicVolume;

    private void Awake() 
    {
        i = this;    
    }

    private void Start() 
    {
        musicVolume = musicPlayer.volume;
    }

    public void PlayMusic(AudioClip musicClip, bool isLooping=true, bool fade=false)
    {
        if(musicClip == null) return;
        StartCoroutine(PlayMusicAsync(musicClip, isLooping, fade));
    }

    IEnumerator PlayMusicAsync(AudioClip musicClip, bool isLooping, bool fade)
    {
        if(fade) yield return musicPlayer.DOFade(0, fadeTime).WaitForCompletion();

        musicPlayer.clip = musicClip;
        musicPlayer.loop = isLooping;
        musicPlayer.Play();

        if(fade) yield return musicPlayer.DOFade(musicVolume, fadeTime).WaitForCompletion();


    }
}
