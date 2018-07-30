using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static AudioClip dealCardSound, takeDamageSound, moveOnBoardSound, stealKidneySound, dieSound, pressEndTurnButtonSound;
    static AudioSource audioSrc;
	// Use this for initialization
	void Start () {
        dealCardSound = Resources.Load<AudioClip>("dealCardSound");
        takeDamageSound = Resources.Load<AudioClip>("takeDamageSound");
        moveOnBoardSound = Resources.Load<AudioClip>("moveOnBoardSound");
        stealKidneySound = Resources.Load<AudioClip>("stealKidneySound");
        dieSound = Resources.Load<AudioClip>("dieSound");
        pressEndTurnButtonSound = Resources.Load<AudioClip>("pressEndTurnButtonSound");

        audioSrc = GetComponent<AudioSource>();
	}
	
    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "dealCardSound":
                audioSrc.PlayOneShot(dealCardSound);
                break;
            case "takeDamageSound":
                audioSrc.PlayOneShot(takeDamageSound);
                break;
            case "moveOnBoardSound":
                audioSrc.PlayOneShot(moveOnBoardSound);
                break;
            case "stealKidneySound":
                audioSrc.PlayOneShot(stealKidneySound);
                break;
            case "dieSound":
                audioSrc.PlayOneShot(dieSound);
                break;
            case "pressEndTurnButtonSound":
                audioSrc.PlayOneShot(pressEndTurnButtonSound);
                break;
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
