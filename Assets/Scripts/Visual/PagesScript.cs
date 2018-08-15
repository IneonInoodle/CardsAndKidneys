using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PagesScript : MonoBehaviour {

    [SerializeField]
    public int CurrentImageNr = 1;
    public Image MainImage;
    public Sprite[] Images;
    void Awake ()
    {
        MainImage.sprite = Images[0];
    }
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }
    public void NextImage()
    {
        MainImage.sprite = Images[CurrentImageNr + 1];
        CurrentImageNr++;
    }
    public void PreviousImage()
    {
        MainImage.sprite = Images[CurrentImageNr - 1];
        CurrentImageNr--;
    }
}
