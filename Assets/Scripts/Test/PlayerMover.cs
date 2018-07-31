using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public class PlayerMover : MonoBehaviour {
    public PlayerManager playerManager;

    public Vector2 destination;
    public bool isMoving = false;

    public float moveSpeed;
    public float delay = 0;

    BoardManager boardManager;

    void Awake()
    {
        boardManager = BoardManager.Instance;
    }

    public bool isValidMove(Point p)
    {
        return false;
    }
    

public void setPlayerArrows(arrows arrowz)
    {   
       playerManager.arrows = arrowz;

       if (playerManager.myCardManager != null)
        {
            playerManager.myCardManager.updateArrows(playerManager.arrows);
        }
    }

    public void Move(Vector2 direction, float delayTime = 0.25f) //checks arrows, and then decides what type of move (3 choices) to make
    {
        
        if (playerManager.ActionPoints == 0)
        return;

        if (!((playerManager.arrows & arrows.Up) == arrows.Up && direction.y == -1     ||
            (playerManager.arrows & arrows.Down) == arrows.Down && direction.y == 1  ||
            (playerManager.arrows & arrows.Left) == arrows.Left && direction.x == -1 ||
            (playerManager.arrows & arrows.Right) == arrows.Right && direction.x == 1 
            ))
        {
            return;
        }  

        Debug.Log(playerManager.point.X + " " + playerManager.point.Y);
        Debug.Log((int)direction.x + " " + (int)direction.y);

        Point des = new Point(playerManager.point.X + (int)direction.x, playerManager.point.Y + (int)direction.y);
        
        // check if we are in the endzone
        if (playerManager.point.Y == -1) // moving out of endzone top
        {
            des.Y = 0; // if we pressed left or right, need to manually fixed our des.y value 
            Debug.Log(des.X + " " + des.Y);
            StartCoroutine(MoveOutOfEndzone(des));

        } else if (playerManager.point.Y == 2) //moving out of endzone bottom
        {
            des.Y = 1; // if we pressed left or right, need to manually fixed our des.y value
            StartCoroutine(MoveOutOfEndzone(des));
        } else if (des.Y == -1) // check if move endzone top
        {
            StartCoroutine(MoveIntoEndzone(boardManager.Top));
        } else if (des.Y == 2) // ,move to endzone bottom
        {
            StartCoroutine(MoveIntoEndzone(boardManager.Bottom));
        } else // move on board
        {
            Debug.Log(des.X + " " + des.Y);
            StartCoroutine(MoveOnBoard(des, 0.2f));
        }

    }

    IEnumerator MoveOnBoard(Point des, float delay)
    {   
        Debug.Log("moveOnBoard");
        OneCardManager desCard;
        Vector2 newPos; 

        if (boardManager.FindCardAtPoint(des) != null)
        {
            SoundManager.PlaySound("dealCardSound");
            desCard = boardManager.FindCardAtPoint(des);
            newPos = desCard.transform.position;
            if (desCard.cardAsset.Damage != 0)
            {
                isMoving = true;
                

                
                //setPlayerArrows(desCard);
                
                boardManager.AddEmptySlot(playerManager.point);
                setPlayerArrows(boardManager.FindCardAtPoint(des).arrows);

                
                

                playerManager.myCardManager.transform.DOMove(newPos, 0.5f);
                playerManager.point = des;
                playerManager.myCardManager.point = des;

                

                playerManager.ActionPoints--;
                playerManager.PickUpKidneyFromBoard();

                // need this order, dont change
                boardManager.DeleteCard(desCard);
                boardManager.RemoveEmptySlot(des);
                playerManager.takeDamage(desCard); // take damage from deleted card?
                

                yield return new WaitForSeconds(0.5f);
                isMoving = false;
                
            }
        }
    }
   
    public IEnumerator MoveIntoEndzone(GameObject des)
    {
        SoundManager.PlaySound("dealCardSound");
        Debug.Log("moveintoendzone");
        //needs to be coroutinetm
        isMoving = true;
        Debug.Log("moveintoendzone");
        playerManager.Doctor.transform.position = des.transform.position; // move doctor image
        playerManager.Doctor.SetActive(true); // make visable


        Debug.Log("moveintoendzone");
        // delete player card

        if (des.name == "Top")
        {
            playerManager.point.X = 1;
            playerManager.point.Y = -1;
            playerManager.myLocation = location.top;
            playerManager.arrows = arrows.Down | arrows.Left | arrows.Right;
        }
        // if pressed down
        if (des.name == "Bottom")
        {
            Debug.Log("moveintoendzone");
            playerManager.point.X = 1;
            playerManager.point.Y = 2;
            playerManager.myLocation = location.bottom;
            playerManager.arrows = arrows.Up | arrows.Left | arrows.Right;
        }

            if (playerManager.myLocation == playerManager.mySide)
            {
                
                playerManager.CaptureKidney(); // capture kidney
            }

        Debug.Log("fuck you");
        boardManager.DeleteCard(playerManager.myCardManager);
        yield return new WaitForSeconds(0.5f);

        

        playerManager.myCardManager = null;
        playerManager.ActionPoints = 0;

        isMoving = false;
    }

    public IEnumerator MoveOutOfEndzone(Point des) // should be done and working 
    {
        arrows temp;
        OneCardManager fieldCardDes;

        Debug.Log("moveoutofendzone");
        if (boardManager.FindCardAtPoint(des) != null)
        {
            SoundManager.PlaySound("dealCardSound");
            isMoving = true;
            fieldCardDes = boardManager.FindCardAtPoint(des);
            playerManager.Doctor.SetActive(false);
            playerManager.PortaitGlowImage.enabled = false;

            temp = fieldCardDes.arrows; // deleting card delete le arrows

            //save playermanager things
            playerManager.myPlayerCard = boardManager.CreateCard(des, playerManager.PlayerCardPrefab, 0.5f);
            playerManager.myCardManager = playerManager.myPlayerCard.GetComponent<OneCardManager>();

            //last 2 lines removed for testing
            //playerManager.ActionPoints = 0;
            //playerManager.turnsOnBoard = 1;
            
            playerManager.point = des;

            if (playerManager.mySide != playerManager.myLocation)
            {
                playerManager.StealKidney();
            }

            playerManager.myLocation = location.board;


            

            setPlayerArrows(temp);
            boardManager.DeleteCard(fieldCardDes);
            boardManager.RemoveEmptySlot(playerManager.point);
            playerManager.takeDamage(fieldCardDes);

            yield return new WaitForSeconds(0.5f);
        }

        isMoving = false; // card finished moving now can take input
        Debug.Log("Done move outof endzone");
        yield return null;
    }

    public void isValidMove()
    {
        // need current location, is on board? or is off board

        // is off board
            
    }
    public void MoveLeft()
    {   //
            Vector2 newPos = new Vector2(-1, 0);
            Move(newPos,0);
      
    }
    public void MoveRight()
    {

            Vector2 newPos = new Vector2(1, 0);
            Move(newPos,0);

    }
    public void MoveUp()
    {

            Vector2 newPos = new Vector2(0,-1);
            Move(newPos, 0);

    }
    public void MoveDown()
    {   
        //problem if the card doenst exist, it has no arrows to check 
        //should not check here

        // need to check that card exists// move from endzone
        
            Vector2 newPos = new Vector2(0, 1);
            Move(newPos, 0);
        
    }
    // Use this for initialization
    void Start () {
        //StartCoroutine(test());
    }
	
    IEnumerator test()
    {
        yield return new WaitForSeconds(1f);
        MoveRight();
        yield return new WaitForSeconds(2f);
        MoveLeft();
        yield return new WaitForSeconds(2f);
        MoveDown();
        yield return new WaitForSeconds(2f);
        MoveUp();
    }
	// Update is called once per frame
	void Update () {


    }
}
