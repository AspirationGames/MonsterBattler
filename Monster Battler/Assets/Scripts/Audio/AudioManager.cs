using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] float fadeTime;

    [SerializeField] List<AudioData> sfxList;
    float musicVolume;

    Dictionary<AudioID, AudioData> sfxDictionary;

    public static AudioManager i;
    private void Awake() 
    {
        i = this;    
    }

    private void Start() 
    {
        musicVolume = musicPlayer.volume;

        sfxDictionary = sfxList.ToDictionary(x => x.id);
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

    public void PlaySFX(AudioClip musicClip)
    {
        if(musicClip == null) return;
        sfxPlayer.PlayOneShot(musicClip); //play one shot won't cancle any other clip being played.
    }

    public void PlaySFX(AudioID audioID)
    {
        if(!sfxDictionary.ContainsKey(audioID)) return;

        AudioData audioData = sfxDictionary[audioID];   
        PlaySFX(audioData.clip);
    }

    
}

public enum AudioID 
{
    UISelect, Hit, Faint, ExpGain
}

[System.Serializable]
public class AudioData
{
    public AudioID id;
    public AudioClip clip;
}
