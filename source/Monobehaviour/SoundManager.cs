using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    AudioSource[] allAudioSources;
    AudioSource shapeReachedAudi;
    AudioSource planeFilledAudio;
    AudioSource gameOverAudio;
    AudioSource rotateAudio;
    AudioSource musicAudioSource;

    void Start ()
    {
        allAudioSources = GetComponents<AudioSource>();
        shapeReachedAudi = allAudioSources[1];
        planeFilledAudio = allAudioSources[2];
        gameOverAudio = allAudioSources[3];
        rotateAudio = allAudioSources[4];
        musicAudioSource = MusicManagerSingleton.Instance.GetComponent<AudioSource>();
        TurnAudioOnOff();
    }

    public void PlayShapeReacheAdui()
    {
        shapeReachedAudi.Play();
    }

    public void PlayPlaneFilledAudio()
    {
        planeFilledAudio.Play();
    }

    public void PlayGameOverAudio()
    {
        gameOverAudio.Play();
    }

    public void PlayRotateAudio()
    {
        rotateAudio.Play();
    }


    public void TurnAudioOnOff()
    {
        bool isAudioOn = Convert.ToBoolean(PlayerPrefs.GetInt(GameMaster.IS_AUDIO_MUTED_KEY));
        
        foreach (AudioSource source in allAudioSources)
        {
            source.mute = !isAudioOn;
        }
        musicAudioSource.mute = !isAudioOn;
    }
}
