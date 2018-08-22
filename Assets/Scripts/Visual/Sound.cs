using UnityEngine.Audio;
using UnityEngine;
using System;
using DG.Tweening;

[System.Serializable]
public class Sound {
    public string name;

    public AudioClip clip;

    [Range(0f,1f)]
    public float volume;
    [Range(.1f,3)]
    public float pitch;
    [HideInInspector]
    public AudioSource source;

    public bool loop;
    public AudioMixer output;
    public AudioMixerGroup outputGroup;

}
