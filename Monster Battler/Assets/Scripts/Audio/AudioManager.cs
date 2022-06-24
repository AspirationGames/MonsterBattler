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
    AudioClip currentMusic;

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
        if(musicClip == null || currentMusic == musicClip) return;

        currentMusic = musicClip;

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

    public void PlaySFX(AudioClip sfxClip, bool pauseMusic=false)
    {
        if(sfxClip == null) return;
        
        
        if(pauseMusic)
        {
            musicPlayer.Pause();
            StartCoroutine(UnpauseMusic(sfxClip.length));
        }

        sfxPlayer.PlayOneShot(sfxClip); //play one shot won't cancle any other clip being played.

    }

    public void PlaySFX(AudioID audioID, bool pauseMusic=false)
    {
        if(!sfxDictionary.ContainsKey(audioID)) return;


        AudioData audioData = sfxDictionary[audioID];   
        PlaySFX(audioData.clip, pauseMusic);
    }

    IEnumerator UnpauseMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        musicPlayer.volume = 0;
        musicPlayer.UnPause();
        musicPlayer.DOFade(musicVolume, fadeTime);
    }
    
}

public enum AudioID 
{
    UISelect, Hit, Faint, ExpGain, ItemObtained, MonsterObtained
}

[System.Serializable]
public class AudioData
{
    public AudioID id;
    public AudioClip clip;
}
