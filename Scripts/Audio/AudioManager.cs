using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour{
    [SerializeField] List<AudioData> sfxList;

    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;

    [SerializeField] float fadeDuration = 0.75f;

    float originalMusicVolume;
    AudioClip currMusic;

    Dictionary<AudioID, AudioData> sfxLookUp;

    public static AudioManager i { get; private set;}

    private void Awake(){
        i = this;
    }

    private void Start(){
        originalMusicVolume = musicPlayer.volume;

        sfxLookUp = sfxList.ToDictionary(x => x.audioID);
    }

    public void PlaySFX(AudioClip clip, bool pauseMusic = false){
        if(clip == null){
            return;
        }

        if(pauseMusic){
            musicPlayer.Pause();
            StartCoroutine(UnPauseMusic(clip.length));
        }

        sfxPlayer.PlayOneShot(clip);
    }

    public void PlaySFX(AudioID audioId, bool pauseMusic = false){
        if(!sfxLookUp.ContainsKey(audioId)){
            return;
        }
        var audioData = sfxLookUp[audioId];
        PlaySFX(audioData.clip, pauseMusic);
    }

    public void PlayMusic(AudioClip clip, bool loop = true, bool fade = false){
        if(clip == null || clip == currMusic ){
            return;
        }

        currMusic = clip;
        StartCoroutine(PlayMusicAsync(clip,loop,fade));
    }

    IEnumerator PlayMusicAsync(AudioClip clip, bool loop, bool fade){
        if(fade){
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();
        }
        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.Play();

        if(fade){
            yield return musicPlayer.DOFade(originalMusicVolume, fadeDuration).WaitForCompletion();
        }
    }

    IEnumerator UnPauseMusic(float delay){
        yield return new WaitForSeconds(delay);

        musicPlayer.volume = 0;
        musicPlayer.UnPause();

        musicPlayer.DOFade(originalMusicVolume, fadeDuration);
    }
}

public enum AudioID{ UISelect, Hit, Faint, ExpGain, ItemObtained, ItemFound, PokemonObtained}

[System.Serializable]
public class AudioData{
    public AudioID audioID;
    public AudioClip clip;
}
