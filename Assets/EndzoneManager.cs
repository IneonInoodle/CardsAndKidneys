using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndzoneManager : MonoBehaviour {

    public Point p;
    public location loc;

    public GameObject CardArrowUp;
    public GameObject CardArrowLeft;
    public GameObject CardArrowRight;
        
    public Material activeMat;
    public Material InactiveMat;

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

    public void setEndZoneArrows()
    {
        int y = loc == location.top ? 0 : 1;

        /*for(int x=0; x<3;x++)
       {
           OneCardManager card = FindCardAtPoint(new Point(x, y));
           if (card == null) continue;
           if ((card.arrows & check) != 0) return true;
       }*/
        
        if (BoardManager.Instance.FindFieldCardAtPoint(new Point(0, y)) != null)
            CardArrowLeft.GetComponent<MeshRenderer>().material = activeMat;
        else
            CardArrowLeft.GetComponent<MeshRenderer>().material = InactiveMat;

        if (BoardManager.Instance.FindFieldCardAtPoint(new Point(1, y)) != null)
            CardArrowUp.GetComponent<MeshRenderer>().material = activeMat;
        else
            CardArrowUp.GetComponent<MeshRenderer>().material = InactiveMat;

        if (BoardManager.Instance.FindFieldCardAtPoint(new Point(2, y)) != null)
            CardArrowRight.GetComponent<MeshRenderer>().material = activeMat;
        else
            CardArrowRight.GetComponent<MeshRenderer>().material = InactiveMat;
    }
}
