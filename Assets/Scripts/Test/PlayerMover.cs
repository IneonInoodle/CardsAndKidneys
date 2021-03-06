﻿using UnityEngine;
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
        if (isMoving)
            return;
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
            MoveOnBoard(des, 0.5f);
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

    public void MoveOnBoard(Point des, float delay)
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

                
                playerManager.point = des;
                playerManager.myCardManager.point = des;
                Debug.Log("actionpoints");
                
                playerManager.PickUpKidneyFromBoard();

                Vector3 v = boardManager.AllSlots[des.Y, des.X].transform.position;

                v.y += 0.3f;
                playerManager.myCardManager.transform.DOMove(v, delay);
                // need this order, dont change
                boardManager.DeleteCard(desCard);


                boardManager.RemoveEmptySlot(des);

                Debug.Log("takingdamage");
                Debug.Log(int.Parse(desCard.DamageText.text));


                //needs to go before take damage, because if take damage kills you, it resets your hp to 10, then this check doesnt work
                if (desCard.cardAsset.Type == CardType.Monster && playerManager.Hp > int.Parse(desCard.DamageText.text))
                {
                    Debug.Log(playerManager.Hp);
                    Debug.Log(int.Parse(desCard.DamageText.text));

                    AudioManager.instance.PlaySound("IceCubeSpill");
                    AudioManager.instance.PlaySound("PlayerDamage");
                    playerManager.raiseAp();
                }

                if (desCard.cardAsset.Type == CardType.Monster)
                {
                    playerManager.takeDamage(int.Parse(desCard.DamageText.text));
                }
                
              
                else if (desCard.cardAsset.Type == CardType.Neutral)
                {
                    AudioManager.instance.PlaySound("PlayerNeutral");
                }
                else if (desCard.cardAsset.Type == CardType.Hp)
                {
                    playerManager.takeDamage(int.Parse(desCard.DamageText.text));
                    AudioManager.instance.PlaySound("PlayerHp");
                }
                // take damage from deleted card?
                
                isMoving = false;
                playerManager.ActionPoints--; // needs to be after take damage
                Debug.Log("updating cards in moveonboard");
                BoardManager.Instance.UpdateCards();
            }
        }
    }
   
    public IEnumerator MoveIntoEndzone(GameObject des)
    {   
        AudioManager.instance.PlaySound("MoveOnBoard");
        //needs to be coroutinetm

        Debug.Log("MoveintoEndzone");
        isMoving = true;
        location loc = playerManager.myLocation;
        if (loc == location.board)
        playerManager.myCardManager.CardFaceGlowObject.SetActive(false);

        playerManager.Doctor.transform.position = des.transform.position; // move doctor image
        playerManager.Doctor.SetActive(true);
        playerManager.Doctor.GetComponent<SpriteRenderer>().enabled =true;// make visable

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
            if (playerManager.Hp > 0)
                playerManager.CaptureKidney(); // capture kidney
            } else
            {
            Debug.Log("tt");
            playerManager.MoveKidneyFromCardToDoctor();
            Debug.Log("tt");
            playerManager.StealKidney();
           
        }


        Sprite sp;
        // if 2 players in one Endzone
        if (GameManager.Instance.getOtherPlayer(playerManager).myLocation == playerManager.myLocation)
        {
            //other player has full portait
            //lay ontop of their portait our half image, need to change out image to half image
            playerManager.Doctor.GetComponent<SpriteRenderer>().sprite = playerManager.PortraitHalf;
            GameManager.Instance.getOtherPlayer(playerManager).Doctor.GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            //make sure out image is full image
            playerManager.Doctor.GetComponent<SpriteRenderer>().sprite = playerManager.PortraitFull;
            //playerManager.Doctor.GetComponent<SpriteRenderer>().sortingOrder = -3;
        }

        if (playerManager.myLocation == location.top)
        {
            this.GetComponentInChildren<SpriteMask>().sprite = GameManager.Instance.TopPortraitMask;
        } else
        {
            this.GetComponentInChildren<SpriteMask>().sprite = GameManager.Instance.BottomPortraitMask;
        }

        if (loc == location.board) // if player was in endzone, they have no card to delete
        {
            Debug.Log("here");
            Debug.Log(playerManager.myCardManager.point.X + " " + playerManager.myCardManager.point.Y);
            Debug.Log(playerManager.point.X + " " + playerManager.point.Y);

            playerManager.myPlayerCard.transform.DOMove(des.transform.position, 3f);

            boardManager.DeleteCard(playerManager.myCardManager);
            Debug.Log("here");
        }
        
        yield return new WaitForSeconds(0.5f);

        playerManager.myCardManager = null;
        Debug.Log("actionpoints");
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
                if (GameManager.Instance.getOtherPlayer(playerManager).myLocation == playerManager.myLocation)
                {
                    GameManager.Instance.getOtherPlayer(playerManager).Doctor.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.getOtherPlayer(playerManager).PortraitFull;
                    GameManager.Instance.getOtherPlayer(playerManager).Doctor.GetComponent<SpriteRenderer>().enabled = true;
                }

                playerManager.button.interactable = true;
                SoundManager.PlaySound("DealCard");
                isMoving = true;
                fieldCardDes = boardManager.FindFieldCardAtPoint(des);
                playerManager.Doctor.SetActive(false);
                playerManager.PortaitGlowObject.SetActive(false);

                temp = fieldCardDes.arrows; // deleting card delete le arrows

                //save playermanager things
                playerManager.myPlayerCard = boardManager.CreateCard(des, boardManager.FieldCardPrefab, playerManager.Doctor, playerManager.CharacterAsset.playerCardAsset, 0.5f).gameObject;
                playerManager.myCardManager = playerManager.myPlayerCard.GetComponent<OneCardManager>();

                //last 2 lines removed for testing
                Debug.Log("actionpoints");
                
                Debug.Log(playerManager.ActionPoints);
                Debug.Log("des");
                Debug.Log(des.X + " " + des.Y);
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
                playerManager.PickUpKidneyFromBoard(); // try to pickup le kidney
                setPlayerArrows(temp);
                boardManager.DeleteCard(fieldCardDes);
                boardManager.RemoveEmptySlot(playerManager.point);

                if (damage != 0)
                {
                    Debug.Log("DAMAGE");
                    Debug.Log(damage);

                    if (fieldCardDes.cardAsset.Type == CardType.Monster)
                    {

                        if (playerManager.Hp > 0 && playerManager.Hp > damage)
                        {
                            Debug.Log(playerManager.Hp);
                            Debug.Log(damage);
                            AudioManager.instance.PlaySound("IceCubeSpill");
                            AudioManager.instance.PlaySound("PlayerDamage");
                            playerManager.raiseAp();
                        }
                        
                    } 

                    playerManager.takeDamage(damage);
                }



                if (fieldCardDes.cardAsset.Type == CardType.Neutral)
                {
                    AudioManager.instance.PlaySound("PlayerNeutral");
                }
                else if (fieldCardDes.cardAsset.Type == CardType.Hp)
                {
                    AudioManager.instance.PlaySound("PlayerHp");

                }

                playerManager.ActionPoints--;

            }
        }
         // card finished moving now can take input
        yield return new WaitForSeconds(0.5f); 
        isMoving = false;// need to add a fucking state manager god damn this is ugly
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
