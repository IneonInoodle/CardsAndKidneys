using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using UnityEngine.UI;
using UnityEditor;
using CommandPattern;

public class AudioManager : MonoBehaviour
{
    //TODO load player sounds from Character Assets

    public static AudioManager instance;

    // Player Unspecific Sounds

    public Sound[] OneOffSounds;
    public Sound[] DealCardSounds;
    public Sound[] CardFlipSounds;
    public Sound[] SplatSounds;
    public Sound[] IceCubesSounds;
    public Sound[] MoveOnBoardSounds;
    public Sound[] StealKidneySounds;

    private Sound[][] UnspecificSounds;

    // Player Specific Sounds

    private Sound[][] player1Sounds;
    private Sound[][] player2Sounds;

    // Audio Mixer

    public AudioMixerGroup MasterMixer;
    public AudioMixer MusicMixer;
    public AudioMixer SoundEffects;

    [Range(-80f, 20f)]
    public float Volume;

    public Slider MusicVolumeSlider;
    public Slider SoundEffectsVolumeSlider;

    public void SetMusicVolume(float volume)
    {
        MusicMixer.SetFloat("volume", volume);
    }

    public void GetEffectsVolume()
    {       
        MusicMixer.GetFloat("volume", out Volume);
        MusicVolumeSlider.value = Volume;
    }

    public void SetEffectVolume(float volume)
    {
        SoundEffects.SetFloat("volume", volume);
    }

    public void GetMusicVolume()
    {
        SoundEffects.GetFloat("volume", out Volume);
        SoundEffectsVolumeSlider.value = Volume;
    }

    public Sound[][] getUnspecificSounds()
    {
        //TODO addme
        Sound[][] s;
        s = new Sound[100][];

        s[0] = OneOffSounds;
        s[1] = DealCardSounds;
        s[2] = SplatSounds;
        s[3] = IceCubesSounds;
        s[4] = MoveOnBoardSounds;
        s[5] = StealKidneySounds;
        s[6] = CardFlipSounds;

        return s;
    }

    public void loadPlayerSoundsFromAssets()
    {
        
        //TODO addme to load from charAsset
        foreach (PlayerManager p in GameManager.Instance.players)
        {   
            Sound[][] s;
            s = new Sound[100][];

            Debug.Log("loadingplayerSoundsFromAssets");

            s[0] = p.CharacterAsset.CaptureSounds;
            s[1] = p.CharacterAsset.DamageSounds;
            s[2] = p.CharacterAsset.HpupSounds;
            s[3] = p.CharacterAsset.KidneyStealSounds;
            s[4] = p.CharacterAsset.LinesSounds;
            s[5] = p.CharacterAsset.NeutralSounds;

            if (p.mySide == location.top)
            {   
                player1Sounds = s;
            }
            else if (p.mySide == location.bottom)
            {
                player2Sounds = s;
            }
        }
    }
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {

            return;
        }

        // fill Sounds Arrays with Sounds
        UnspecificSounds = getUnspecificSounds();
    }

    private void createSoundSourceAndPlay(Sound s) // creates source and attaches it to gameObject
    {
        if (s != null)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputGroup;
            s.source.playOnAwake = s.PlayOnAwake;

            s.source.Play();
            StartCoroutine(deleteSource(s, s.clip.length));//coroutine waits for sound to finish playing then deletes it
        }
    }

    IEnumerator deleteSource(Sound s, float delay)
    {
        yield return new WaitForSeconds(delay);
        s.source = null;
    }

    public Sound getRandomPlayerSound(string name)
    {
        PlayerManager p = GameManager.Instance.CurrentPlayerTurn;
        Sound[][] ss;
        ss = new Sound[100][];
        Sound s = null;

        if (name == null) return s;
        // Decide Which Array To Search
        if (p.mySide == location.top)
        {
            ss = player1Sounds;
        }
        else if (p.mySide == location.bottom)
        {
            ss = player2Sounds;
        } 

        switch (name)
        {
            default:
                for (int i = 0; i < ss.Length; i++)
                {   
                    if (ss[i] != null)
                    {
                        s = Array.Find(ss[i], sound => sound.name.Contains(name));
                        if (s != null)
                        { //set to random inside of Array
                            s = ss[i][UnityEngine.Random.Range(0, ss[i].Length)];
                            break;
                        }
                    }
                }
                break;
        }
        return s;

    }
    public Sound getRandomSound(string name)
    {
        Sound s = null;
        switch (name)
        {
            //BoardSounds Nonplayer Specific
            default:
                for (int i = 0; i < UnspecificSounds.Length; i++)
                {   
                    if (UnspecificSounds[i] != null)
                    {
                        s = Array.Find(UnspecificSounds[i], sound => sound.name.Contains(name));
                        if (s != null)
                        { //set to random inside of Array
                            s = UnspecificSounds[i][UnityEngine.Random.Range(0, UnspecificSounds[i].Length)];
                            break;
                        }
                    }
                }
                break;
        }
        return s;
    }
    public void PlaySound(string name)
    {
        Debug.Log(name);
        Sound s = null;
        switch (name)
        {
            //BoardSounds Nonplayer Specific
            default:
                for (int i = 0; i < UnspecificSounds.Length; i++)
                {   
                    if (UnspecificSounds[i] != null)
                    {
                        s = Array.Find(UnspecificSounds[i], sound => sound.name == name);
                        if (s != null) break;
                    }
                }
                break;
            case "CardFlip": //done
                Debug.Log("inhere");
                s = getRandomSound(name);
                break;
            case "DealCard"://done
                s = getRandomSound(name);
                break;

            case "IceCubeSpill"://done
                s = getRandomSound(name);
                break;
            case "Splat"://done
                s = getRandomSound(name);
                break;
            case "MoveOnBoard"://done
                s = getRandomSound(name);
                break;
            case "StealKidney"://done
                s = getRandomSound(name);
                break;
            
            //Player Specific Sounds
            //TODO "hpUp": addme
            //TODO "dropKidney" addme          
            //TODO"stealKidneySound" addme
            //TODO "pressEndTurnButtonSound"  addme

            case "PlayerCapture":
                s = getRandomPlayerSound(name);
                break;

            case "PlayerDamage":
                s = getRandomPlayerSound(name);
                break;

            case "PlayerHp":
                s = getRandomPlayerSound(name);
                break;

            case "PlayerStealKidney":
                s = getRandomPlayerSound(name);
                break;

            case "PlayerIntroLine":
                s = getRandomPlayerSound(name);
                break;

            case "PlayerNeutral":
                s = getRandomPlayerSound(name);
                break;
        }

        if (s == null) {
            Debug.LogError("No Sound Found For: " + name);
            return;
        } else
        {
            Debug.Log(s.name);
        }

        createSoundSourceAndPlay(s);
    }
}
