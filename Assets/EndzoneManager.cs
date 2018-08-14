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

    public void setArrows()
    {
        Debug.Log("setarrows");
        int y = loc == location.top ? 0 : 1;

        /*for(int x=0; x<3;x++)
       {
           OneCardManager card = FindCardAtPoint(new Point(x, y));
           if (card == null) continue;
           if ((card.arrows & check) != 0) return true;
       }*/
        CardArrowRightImage.color = Color.white;
        if (BoardManager.Instance.FindFieldCardAtPoint(new Point(0, y)) != null)
            CardArrowLeftImage.color = Color.white;
        else
            CardArrowLeftImage.color = Color.black;

        if (BoardManager.Instance.FindFieldCardAtPoint(new Point(1, y)) != null)
            CardArrowUpImage.color = Color.white;
        else
            CardArrowUpImage.color = Color.black;

        if (BoardManager.Instance.FindFieldCardAtPoint(new Point(2, y)) != null)
            CardArrowRightImage.color = Color.white;
        else
            CardArrowRightImage.color = Color.black;
    }
}
