using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
//using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {
    public Sound[] OneOffsounds;
    public Sound[] dealCardSounds;
    public Sound[] splatSounds;
    public Sound[] iceCubesSounds;
    public Sound[] doctor1Sounds;
    public Sound[] doctor2Sounds;
    public AudioMixerGroup masterMixer;
    public AudioMixer mainMixer;
    [Range(-80f, 20f)]
    public float volume;
    public Slider mslider;
    public static AudioManager instance;
    
    private Sound[][] AllSounds;

    // Use this for initialization

    public void SetVolume(float volume)
    {
        mainMixer.SetFloat("volume", volume);
    }

    public void GetVolume()
    {
        mainMixer.GetFloat("volume", out volume);
        mslider.value = volume;        
    }

    public void Start()
    {
       //mainMixer.SetFloat("volume", volume);
    }

    public void Update()
    {
      //  GetVolume(volume);
    }
    void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        AllSounds = new Sound[6][];
        AllSounds[0] = OneOffsounds;
        AllSounds[1] = dealCardSounds;
        AllSounds[2] = splatSounds;
        AllSounds[3] = iceCubesSounds;
        AllSounds[4] = doctor1Sounds;
        AllSounds[5] = doctor2Sounds;

        for (int i = 0; i < AllSounds.Length; i++)
        {
            for (int j = 0; j < AllSounds[i].Length; j++)
            {

                AllSounds[i][j].source = gameObject.AddComponent<AudioSource>();
                AllSounds[i][j].source.clip = AllSounds[i][j].clip;

                AllSounds[i][j].source.volume = AllSounds[i][j].volume;
                AllSounds[i][j].source.pitch = AllSounds[i][j].pitch;
                AllSounds[i][j].source.loop = AllSounds[i][j].loop;
                AllSounds[i][j].source.outputAudioMixerGroup = masterMixer;
            }
        }
    }

    // Update is called once per frame

   /* public static void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        audiomanager.
    }*/
    public void Play(string name)
    {
        Sound s = null;

        switch (name)
        {
            case "dealCardSound"://done
                s = AllSounds[1][UnityEngine.Random.Range(0, dealCardSounds.Length)];
                break;
            //"hpUp": addme
            case "iceCube"://done
                s = AllSounds[1][UnityEngine.Random.Range(0, iceCubesSounds.Length)];
                break;
            case "splat"://done
                s = AllSounds[2][UnityEngine.Random.Range(0, splatSounds.Length)]; //Anis this is broken here please fix
                break;
            //"dropKidney" addme          
            //"stealKidneySound" addme
            //"pressEndTurnButtonSound"  addme
            default:
                for (int i = 0; i < AllSounds.Length; i++)
                {
                    s = Array.Find(AllSounds[i], sound => sound.name == name);
                    if (s != null) break;
                }
                break;
        }

        /*
        if (name = "dealCardSound")
        {
            s = dealCardSounds[0, Random.Range.dealcardsounds.length];
            
        } else
        {
            for (int i = 0; i < AllSounds.Length; i++)
            {
                s = Array.Find(AllSounds[i], sound => sound.name == name);
                if (s != null) break;
            }

        }*/




        if (s == null) return;
        s.source.Play();
    }
}
