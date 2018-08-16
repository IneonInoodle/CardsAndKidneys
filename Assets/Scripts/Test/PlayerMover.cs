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

    public void MoveToPoint(Point des, float delayTime = 0.25f)
    {
        Debug.Log("moveToPoint");
        // check if we are in the endzone
        if (playerManager.point.Y == -1) // moving out of endzone top
        {
            des.Y = 0; // if we pressed left or right, need to manually fixed our des.y value 
            StartCoroutine(MoveOutOfEndzone(des));

        }
        else if (playerManager.point.Y == 2) //moving out of endzone bottom
        {
            des.Y = 1; // if we pressed left or right, need to manually fixed our des.y value
            StartCoroutine(MoveOutOfEndzone(des));
        }
        else if (des.Y == -1) // check if move endzone top
        {
            StartCoroutine(MoveIntoEndzone(boardManager.Top));
        }
        else if (des.Y == 2) // ,move to endzone bottom
        {
            StartCoroutine(MoveIntoEndzone(boardManager.Bottom));
        }
        else // move on board
        {
            Debug.Log(des.X + " " + des.Y);
            StartCoroutine(MoveOnBoard(des, 0.2f));
        }

    }
    public void MoveKeyBoard(Vector2 direction) //checks arrows, and then decides what type of move (3 choices) to make
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

        Point des = new Point(playerManager.point.X + (int)direction.x, playerManager.point.Y + (int)direction.y);

        MoveToPoint(des, 0.25f);

    }

    IEnumerator MoveOnBoard(Point des, float delay)
    {   
        OneCardManager desCard;
        Vector2 newPos; 

        if (boardManager.FindFieldCardAtPoint(des) != null)
        {   

            
            desCard = boardManager.FindFieldCardAtPoint(des);
            //newPos = desCard.transform.localosition;
            if (desCard.cardAsset.Type != CardType.Player)
            {
                SoundManager.PlaySound("dealCardSound");
                isMoving = true;
                playerManager.button.interactable = true;



                //setPlayer


                //(desCard);

                boardManager.AddEmptySlot(playerManager.point);
                setPlayerArrows(boardManager.FindFieldCardAtPoint(des).arrows);

                playerManager.myCardManager.transform.DOMove(boardManager.AllSlots[des.Y, des.X].transform.position, delay);
                playerManager.point = des;
                playerManager.myCardManager.point = des;

                playerManager.ActionPoints--;
                playerManager.PickUpKidneyFromBoard();

                // need this order, dont change
                boardManager.DeleteCard(desCard);
                boardManager.RemoveEmptySlot(des);

                Debug.Log("takingdamage");
                Debug.Log(int.Parse(desCard.DamageText.text));

                playerManager.takeDamage(int.Parse(desCard.DamageText.text));
                if (desCard.cardAsset.Type == CardType.Monster && playerManager.Hp > 0)
                {
                    playerManager.raiseAp();
                }
                

                // take damage from deleted card?
                
                yield return new WaitForSeconds(0.5f);
                BoardManager.Instance.UpdateCards();
                isMoving = false;
                
            }
        }
    }
   
    public IEnumerator MoveIntoEndzone(GameObject des)
    {
        SoundManager.PlaySound("dealCardSound");
        //needs to be coroutinetm
        isMoving = true;

        playerManager.myCardManager.CardFaceGlowObject.SetActive(false);

        playerManager.Doctor.transform.position = des.transform.position; // move doctor image
        playerManager.Doctor.SetActive(true); // make visable
       
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
            playerManager.point.X = 1;
            playerManager.point.Y = 2;
            playerManager.myLocation = location.bottom;
            playerManager.arrows = arrows.Up | arrows.Left | arrows.Right;
        }

            if (playerManager.myLocation == playerManager.mySide)
            {
                
                playerManager.CaptureKidney(); // capture kidney
            } else
            {
            
            playerManager.MoveKidneyFromCardToDoctor();

            if (playerManager.mySide != playerManager.myLocation)
            {
                playerManager.StealKidney();
            }
            playerManager.ActionPoints--;
        }


        Sprite sp;
        // if 2 players in one Endzone
        if (GameManager.Instance.getOtherPlayer(playerManager).myLocation == playerManager.myLocation)
        {
            //other player has full portait
            //lay ontop of their portait our half image, need to change out image to half image
            playerManager.Doctor.GetComponent<SpriteRenderer>().sprite = playerManager.PortraitHalf;
            playerManager.Doctor.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        else
        {
            //make sure out image is full image
            playerManager.Doctor.GetComponent<SpriteRenderer>().sprite = playerManager.PortraitFull;
            playerManager.Doctor.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }

        if (playerManager.myLocation == location.top)
        {
            this.GetComponentInChildren<SpriteMask>().sprite = GameManager.Instance.TopPortraitMask;
        } else
        {
            this.GetComponentInChildren<SpriteMask>().sprite = GameManager.Instance.BottomPortraitMask;
        }

        boardManager.DeleteCard(playerManager.myCardManager);
        yield return new WaitForSeconds(0.5f);

        playerManager.myCardManager = null;
        playerManager.ActionPoints--;
        playerManager.button.interactable = true;
        BoardManager.Instance.UpdateCards();
        isMoving = false;
    }

    public IEnumerator MoveOutOfEndzone(Point des) // should be done and working 
    {
        arrows temp;
        OneCardManager fieldCardDes;

        if (boardManager.FindFieldCardAtPoint(des) != null)
        {
            if (boardManager.FindFieldCardAtPoint(des).cardAsset.Type != CardType.Player)
            {
                playerManager.button.interactable = true;
                SoundManager.PlaySound("dealCardSound");
                isMoving = true;
                fieldCardDes = boardManager.FindFieldCardAtPoint(des);
                playerManager.Doctor.SetActive(false);
                playerManager.PortaitGlowObject.SetActive(false);

                temp = fieldCardDes.arrows; // deleting card delete le arrows

                //save playermanager things
                playerManager.myPlayerCard = boardManager.CreateCard(des, boardManager.FieldCardPrefab, playerManager.Doctor, playerManager.playerCardAsset, 0.5f).gameObject;
                playerManager.myCardManager = playerManager.myPlayerCard.GetComponent<OneCardManager>();

                //last 2 lines removed for testing
                playerManager.ActionPoints--;
                playerManager.point = des;

                if (playerManager.playerKidneys.Count < 1)
                {
                    if (playerManager.mySide != playerManager.myLocation)
                    {
                        //playerManager.StealKidney();
                    }
                }
                else
                {
                    playerManager.MoveKidneyFromDoctorToCard();
                }


                playerManager.myLocation = location.board;
                int damage = int.Parse(fieldCardDes.DamageText.text);

                if (damage != 0)
                {
                    Debug.Log("DAMAGE");
                    Debug.Log(damage);
                    playerManager.takeDamage(damage);
                }

                if (fieldCardDes.cardAsset.Type == CardType.Monster)
                {
                    Debug.Log("RaisAP");
                    if (playerManager.Hp > 0)
                        playerManager.raiseAp();
                }


                setPlayerArrows(temp);



                boardManager.DeleteCard(fieldCardDes);
                boardManager.RemoveEmptySlot(playerManager.point);

                
                
                
                
            }
        }
        isMoving = false; // card finished moving now can take input
        BoardManager.Instance.UpdateCards();
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
        MoveKeyBoard(newPos);
      
    }
    public void MoveRight()
    {

            Vector2 newPos = new Vector2(1, 0);
        MoveKeyBoard(newPos);

    }
    public void MoveUp()
    {

            Vector2 newPos = new Vector2(0,-1);
        MoveKeyBoard(newPos);

    }
    public void MoveDown()
    {   
        //problem if the card doenst exist, it has no arrows to check 
        //should not check here

        // need to check that card exists// move from endzone
        
            Vector2 newPos = new Vector2(0, 1);
        MoveKeyBoard(newPos);
        
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
