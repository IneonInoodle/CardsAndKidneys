using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndzoneManager : MonoBehaviour {

    public Point p;
    public location loc;

    public Image CardArrowUpImage;
    public Image CardArrowLeftImage;
    public Image CardArrowRightImage;

    public Image PortraitGlowImage;

    public bool isSelectable = false;
    // Use this for initialization
    void Start () {
        PortraitGlowImage.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnMouseDown()
    {   
        if (isSelectable == true)
        {
            GameManager.Instance.CurrentPlayerTurn.playerMover.MoveToPoint(p);
            isSelectable = false;
        }
    }
}
