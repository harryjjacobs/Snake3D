using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {

    private static Dictionary<string, AudioSource> audioSources;
    private static Dictionary<string, AudioClip> audioClips;

    void Awake()
    {
        Debug.Log("HAI!");
        audioSources = new Dictionary<string, AudioSource>();
        audioClips = new Dictionary<string, AudioClip>();
    }

    public static void RegisterAudioSource(string name, AudioSource audioSource)
    {
        if (audioSources.ContainsKey(name))
        {
            audioSources[name] = audioSource;
        } else
        {
            audioSources.Add(name, audioSource);
        }
    }

    public static void PlaySound(string audioSource, string audioClip, float speed = 1)
    {
        if (audioSources.ContainsKey(audioSource))
        {
            audioSources[audioSource].clip = GetAudioClip(audioClip);
            audioSources[audioSource].pitch = speed;
            audioSources[audioSource].Play();
        }
    }


    static AudioClip GetAudioClip(string name)
    {
        if (!audioClips.ContainsKey(name))
        {
            audioClips.Add(name, Resources.Load<AudioClip>("Audio/" + name));
        }
        return audioClips[name];
    }
}
