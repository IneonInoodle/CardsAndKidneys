using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PagesScript : MonoBehaviour {

    [SerializeField]
    public int CurrentImageNr = 1;
    public TextMeshProUGUI CurrentPage;
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
        CurrentPage.text = CurrentImageNr+1 + "/" + Images.Length;
    }
    public void NextImage()
    {
        if (CurrentImageNr != Images.Length-1) { 
        MainImage.sprite = Images[CurrentImageNr + 1];
        CurrentImageNr++;
        CurrentPage.text = CurrentImageNr + 1 + "/" + Images.Length;
    }
    }
    public void PreviousImage()
    {
        if (CurrentImageNr != 0)
        {
            MainImage.sprite = Images[CurrentImageNr - 1];
            CurrentImageNr--;
            CurrentPage.text = CurrentImageNr + 1 + "/" + Images.Length;
        }
    }
}
