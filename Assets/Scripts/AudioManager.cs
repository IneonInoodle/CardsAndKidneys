using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour {
    public Sound[] OneOffsounds;
    public Sound[] dealCardSounds;
    public Sound[] splatSounds;
    public Sound[] iceCubesSounds;
    public Sound[] doctor1Sounds;
    public Sound[] doctor2Sounds;


    public static AudioManager instance;

    private Sound[][] AllSounds;

    // Use this for initialization
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
            }
        }


        


       
    }

    // Update is called once per frame
    public void Play(string name)
    {
        Sound s = null;
        for (int i = 0; i < AllSounds.Length; i++)
        {
            s = Array.Find(AllSounds[i], sound => sound.name == name);
            if (s != null) break;
        }

        if (s == null) return;
        s.source.Play();
    }
}
