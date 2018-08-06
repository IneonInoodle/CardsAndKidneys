using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static AudioClip cardPlace1, cardPlace2, cardPlace3, cardPlace4,
        Meat_Slice_1, Meat_Slice_2, Meat_Slice_3, Meat_Slice_4, Meat_Slice_5,
        cardFlip,
        diceThrow1, diceThrow2, diceThrow3, diceThrow4,
        dropKidney,
        kidneySplat1, kidneySplat2, kidneySplat3,
        hpUp,

        dealCardSound, takeDamageSound, moveOnBoardSound, stealKidneySound, dieSound, pressEndTurnButtonSound;

    public static AudioClip[] dealCardSounds;
    public static AudioClip[] stealKidneySounds;
    public static AudioClip[] kidneySplats;
    public static AudioClip[] iceCubes;

    static AudioSource audioSrc;
    // Use this for initialization

    private void Awake()
    {
        pressEndTurnButtonSound = Resources.Load<AudioClip>("Slam_05");
    }
    void Start () {

        hpUp = Resources.Load<AudioClip>("dustyroom_multimedia_select_digital_button");  
        dropKidney = Resources.Load<AudioClip>("cartoon_slide_whistle_ascend_then_descend");

        cardPlace1 = Resources.Load<AudioClip>("cardPlace1");
        cardPlace2 = Resources.Load<AudioClip>("cardPlace2");
        cardPlace3 = Resources.Load<AudioClip>("cardPlace3");
        cardPlace4 = Resources.Load<AudioClip>("cardPlace4");

        dealCardSounds = new AudioClip[] { cardPlace1, cardPlace2, cardPlace3, cardPlace4 };

        Meat_Slice_1 = Resources.Load<AudioClip>("Meat_Slice_01");
        Meat_Slice_2 = Resources.Load<AudioClip>("Meat_Slice_02");
        Meat_Slice_3 = Resources.Load<AudioClip>("Meat_Slice_03");
        Meat_Slice_4 = Resources.Load<AudioClip>("Meat_Slice_04");
        Meat_Slice_5 = Resources.Load<AudioClip>("Meat_Slice_05");

        stealKidneySounds = new AudioClip[] { Meat_Slice_1, Meat_Slice_2, Meat_Slice_3, Meat_Slice_4, Meat_Slice_5 };

        kidneySplat1 = Resources.Load<AudioClip>("Splat_01");
        kidneySplat2 = Resources.Load<AudioClip>("Splat_02");
        kidneySplat3 = Resources.Load<AudioClip>("Splat_03");

        kidneySplats = new AudioClip[] { kidneySplat1, kidneySplat2, kidneySplat3};

        diceThrow1 = Resources.Load<AudioClip>("diceThrow1");
        diceThrow2 = Resources.Load<AudioClip>("diceThrow2");
        diceThrow3 = Resources.Load<AudioClip>("diceThrow3");
        diceThrow4 = Resources.Load<AudioClip>("diceThrow4");

        iceCubes = new AudioClip[] { diceThrow1, diceThrow2, diceThrow3, diceThrow4 };


        //dealCardSound = Resources.Load<AudioClip>("dealCardSound");
        takeDamageSound = Resources.Load<AudioClip>("Medical_Syringe_06");
        cardFlip = Resources.Load<AudioClip>("cardOpenPackage2");





        moveOnBoardSound = Resources.Load<AudioClip>("moveOnBoardSound");
        dieSound = Resources.Load<AudioClip>("DIE");
        

        audioSrc = GetComponent<AudioSource>();
	}
	
    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "hpUp":
                audioSrc.PlayOneShot(hpUp);
            break;

            case "iceCube":
                audioSrc.PlayOneShot(iceCubes[Random.Range(0, iceCubes.Length)]);
                break;
            case "splat":
                audioSrc.PlayOneShot(kidneySplats[Random.Range(0, kidneySplats.Length)]);
                break;
            case "dropKidney":
                audioSrc.PlayOneShot(dropKidney);
                break;

            case "dealCardSound":
                audioSrc.PlayOneShot(dealCardSounds[Random.Range(0, dealCardSounds.Length)]);
                break;
            case "takeDamageSound":
                audioSrc.PlayOneShot(takeDamageSound);
                break;
            case "moveOnBoardSound":
                audioSrc.PlayOneShot(moveOnBoardSound);
                break;
            case "stealKidneySound":
                audioSrc.PlayOneShot(stealKidneySounds[Random.Range(0, stealKidneySounds.Length)]);
                break;
            case "cardFlip":
                audioSrc.PlayOneShot(cardFlip);
                break;
            case "dieSound":
                audioSrc.PlayOneShot(dieSound);
                break;
            case "pressEndTurnButtonSound":
                //annoying bug here
                
                break;
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
