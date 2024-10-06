using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio Event")]
public class AudioEvent : ScriptableObject
{
    public AudioClip[] Clips;
    public float MinPitch;
    public float MaxPitch;
    public float MinVolume;
    public float MaxVolume;

    public void Play(AudioSource audioSource)
    {
        AudioClip clip = Clips[Random.Range(0, Clips.Length)];
        float pitch = Random.Range(MinPitch, MaxPitch);
        float volume = Random.Range(MinVolume, MaxVolume);
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.Play();
    }
}