using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;
public class VidPlay : MonoBehaviour {

    public RawImage image;
    public VideoClip videoToPlay;
    public RawImage Hole;
    public RawImage door;
    public RawImage Kidney;
    public RawImage Texture;
    private VideoPlayer videoPlayer;
    private VideoSource videoSource;
    public Rigidbody camerra;
    private AudioSource audioSource;

    void Start()
    {
        Application.runInBackground = true;
        StartCoroutine(playVideo());
    }

    IEnumerator playVideo()
    {
        //Add VideoPlayer to the GameObject
        videoPlayer = gameObject.AddComponent<VideoPlayer>();

        //Add AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();

        //Disable Play on Awake for both Video and Audio
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;

        //We want to play from video clip not from url
        videoPlayer.source = VideoSource.VideoClip;

        //Set Audio Output to AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        videoPlayer.clip = videoToPlay;
        videoPlayer.Prepare();
        //WaitForSeconds waitTime = new WaitForSeconds(5);
        //Wait until video is prepared
       while (!videoPlayer.isPrepared)
        {
            Debug.Log("Preparing Video");
            yield return null;
        }

        Debug.Log("Done Preparing Video");
        image.enabled = true;
        //Assign the Texture from Video to RawImage to be displayed
        image.texture = videoPlayer.texture;
        //AudioManager.instance.PlaySound("GameIntro"); //idk what audio this was
        //Play Video
        videoPlayer.Play();

        //Play Sound
        audioSource.Play();

        Debug.Log("Playing Video");
        while (videoPlayer.isPlaying)
        {
            Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));
            yield return null;
        }
        
        Hole.enabled = true;
        door.enabled = true;
        Kidney.enabled = true;
        Texture.enabled = true;
        image.enabled = false;
        Debug.Log("Done Playing Video");
    }
    public void NextScene()
    {
        Kidney.transform.DOMoveZ(-50f, 6);
        Texture.transform.DOMoveZ(50f, 6);
        StartCoroutine(LoadNewScene());
    }
    IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(1.5f);
        Camera.main.transform.DOMove(new Vector3(0.456f, -85.54f, 5.456f), 3f);
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("MainMenu");
    }   
}
