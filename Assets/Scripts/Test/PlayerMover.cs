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

    public void setPlayerArrows(OneCardManager c)
    {   
        if (c != null) { 
            playerManager.myCardManager.CardArrowDownImage.enabled = c.CardArrowDownImage.isActiveAndEnabled;
            playerManager.myCardManager.CardArrowUpImage.enabled = c.CardArrowUpImage.isActiveAndEnabled;
            playerManager.myCardManager.CardArrowLeftImage.enabled = c.CardArrowLeftImage.isActiveAndEnabled;
            playerManager.myCardManager.CardArrowRightImage.enabled = c.CardArrowRightImage.isActiveAndEnabled;
        }
    }

    public void Move(Vector2 direction, float delayTime = 0.25f) //checks arrows, and then decides what type of move (3 choices) to make
    {
        // check if action points this check can be moved

        if (playerManager.ActionPoints == 0)
            return;
        // always called to move person 

        // check player in endzone or not
        //if they press left up or right, see if they can move there
        // check arrows

        // check bottom 3 cards on board


        Debug.Log(playerManager.mySide + " Player");


        if (playerManager.myLocation == location.board) {  //check if field card and check if on field, pressing move sets the card to off the map

            if (playerManager.myCardManager.CardArrowDownImage.enabled == true && direction.x == 1 || //check if down arrow and down direction
              playerManager.myCardManager.CardArrowUpImage.enabled == true && direction.x == -1 || //check arrow up 
              playerManager.myCardManager.CardArrowLeftImage.enabled == true && direction.y == -1 || //check arrow left
              playerManager.myCardManager.CardArrowRightImage.enabled == true && direction.y == 1  //check arrow right
              ) {

                if (playerManager.mySlot.indRow == 0 && direction.x == -1) // moving into top endzone
                {
                    Debug.Log("move to endzone top");
                    StartCoroutine(MoveIntoEnzone(boardManager.Top));
                }
                else if (playerManager.mySlot.indRow == 1 && direction.x == 1)
                {
                    Debug.Log("move to endzone bottom");
                    StartCoroutine(MoveIntoEnzone(boardManager.Bottom));
                } else
                {
                    //if not moving into the enzone, NORMAL MOVE ON BOARD
                    StartCoroutine(MoveOnBoard(direction, delayTime));
                    
                }


            }
        } else //player is in endzone, wanting to move out
        {
            // move out of endzone
            
            StartCoroutine(MoveOutOfEndzone(direction));

            playerManager.MoveKidneyFromDoctorToCard();
            playerManager.PickUpKidneyFromBoard();

        }

        // finished moving, change action points
        //playerManager.myCardManager.ActionPointsText.text = playerManager.actionPoints.ToString() +  "/" + playerManager.turnsOnBoard.ToString(); 
    }

    IEnumerator MoveOnBoard(Vector2 direction, float delayTime)
    {
        OneCardManager CardManagerToDelete;
        isMoving = true;
        
        Vector2 newPos = playerManager.myCardManager.transform.position;
        List<CardSlotManager> Neigbors = playerManager.myCardManager.GetNeighborsSlots();     
       // in center of map
        {   
            //need to check all neighbors, for available moves
            foreach (CardSlotManager t in Neigbors)
            {   
                //does card exist at slot?
                if (t.Card != null)
                {
                    //had to do three shitty loops here
                    // does card at card in slot have row and col equal to our goal
                    if (t.indRow == playerManager.mySlot.indRow + (int)direction.x && t.indCol == playerManager.mySlot.indCol + (int)direction.y)
                    {
                        if (t.Card.GetComponent<OneCardManager>().cardAsset.Damage != 0) // check if field card NOT PLAYER CARD
                        {
                            Debug.Log("movingOnBoard");
                            // add the slot under the player before they move to the empty list
                            boardManager.EmptyCardSlots.Add(playerManager.mySlot);
                            newPos = t.Card.transform.position;
                            playerManager.myPlayerCard.transform.DOMove(newPos, 1f);


                            CardManagerToDelete = t.Card.GetComponent<OneCardManager>();
                            
                            

                            boardManager.AllSlots[playerManager.mySlot.indRow, playerManager.mySlot.indCol].Card = null; // set old slot to null
                            // set Allslots/ card slot to empty
                            playerManager.mySlot = boardManager.AllSlots[t.indRow, t.indCol];
                            playerManager.myCardManager.myCardSlot = playerManager.mySlot; //rip terrible programming here 

                            //boardManager.AllSlots[t.indRow, t.indCol].Card = playerManager.myPlayerCard; // rip
                            // wow delete card
                            Debug.Log("row: " + t.indRow + " t.indCol: " + t.indCol);

                            // TODO - TAKE DAMAGE HERE?

                            

                            
                            playerManager.ActionPoints--;

                            



                            // set slot to new slot
                            boardManager.AllSlots[t.indRow, t.indCol].Card = playerManager.myPlayerCard;
                            //set arrows
                            setPlayerArrows(CardManagerToDelete);
                            playerManager.PickUpKidneyFromBoard();
                            playerManager.takeDamage(CardManagerToDelete);
                            StartCoroutine(CardManagerToDelete.DeleteThisCard());
                        }
                    }
                }
            }
            while (Vector2.Distance(newPos, playerManager.myPlayerCard.transform.position) > 0.01f) // dont contine until object finished moving
            {
                yield return null;
            }
            
            isMoving = false;
            playerManager.myPlayerCard.transform.position = newPos;
        }
    }
    public IEnumerator MoveIntoEnzone(GameObject des)
    {
        Debug.Log("moveintoendzone");
        //needs to be coroutinetm
        isMoving = true;
        playerManager.Doctor.transform.position = des.transform.position; // move doctor image
        playerManager.Doctor.SetActive(true); // make visable

        playerManager.MoveKidneyFromCardToDoctor();

        // add the slot under the player before they move to the empty list
        boardManager.EmptyCardSlots.Add(playerManager.mySlot);
        boardManager.AllSlots[playerManager.mySlot.indRow, playerManager.mySlot.indCol].Card = null; // set old slot to null
        


        //playerManager.myCardManager.myCardSlot = null; //probably dont need this

        // set board allslot[].card to empty  
        
        // if up in top endzone
        //Set correct zone so next time move out of enzone is triggered
        if (des.name == "Top")
        {
            playerManager.myLocation = location.top;
        }
        // if pressed down
        if (des.name == "Bottom")
        {
            playerManager.myLocation = location.bottom;
        }

        StartCoroutine(playerManager.myCardManager.DeleteThisCard()); // change in add delete card command to que

        while (playerManager.myCardManager != null) // wait until card is deleted to continue
        {
            yield return null;
        }
        Debug.Log("move into endzone done");
        isMoving = false;

        playerManager.mySlot = null;
        playerManager.myCardManager = null;

        playerManager.ActionPoints = 0;
    }

    public IEnumerator MoveOutOfEndzone(Vector2 direction)
    {
        Debug.Log("moveoutofendZoneTest");

        isMoving = true;

        //check if in enemy endzone // 
        //string location = playerManager.myLocation.ToString();

        if (playerManager.mySide != playerManager.myLocation)
        {
            playerManager.PickUpKidneyFromEnemy();
        }

        // now need to check what cards are availble for player to move
        // if player in top, need to check top 3 cards
        // if player in low need to check bottom 3 cards

        int row; //row is the "row to check" 1 for bottom row, 0 for top row
        Debug.Log(playerManager.myLocation);
        Debug.Log("gg");
        if (playerManager.myLocation != location.board)
        {
            if (playerManager.myLocation == location.bottom) { row = 1; }
                
            else { row = 0; }
                //if pressed left
            //check card at (0,1)

            // we now check the cards, 
            if (direction.y == -1 && boardManager.AllSlots[row, 0].Card != null) // user pressed left, card check at (0,1) or 1,1
            {
                Debug.Log("moveoutofendZone");
                playerManager.Doctor.SetActive(false); // turn of doctor object
                if (row == 1)
                {
                    Debug.Log("bottom left");
                    // only difference here is that we need to pass in location of "spawn card slots?"
                    playerManager.spawnPlayerCardAtSlot(boardManager.AllSlots[row, 0], playerManager.PlayerCardPrefab, boardManager.Bottom);
                } else
                {
                    Debug.Log("top left");
                    playerManager.spawnPlayerCardAtSlot(boardManager.AllSlots[row, 0], playerManager.PlayerCardPrefab, boardManager.Top);
                }
            }
                // we check the rightmost 2 cards 
                //TODO add function isvalidmove To boarmanager or cardslotmanager
            if (direction.y == 1 && boardManager.AllSlots[row, 2].Card != null)
            {

                Debug.Log("moveoutofendZone");
               
                if (row == 1)
                {
                    Debug.Log("top right");
                    playerManager.spawnPlayerCardAtSlot(boardManager.AllSlots[row, 2], playerManager.PlayerCardPrefab, boardManager.Bottom);
                }
                else
                {
                    Debug.Log("top left");
                    playerManager.spawnPlayerCardAtSlot(boardManager.AllSlots[row, 2], playerManager.PlayerCardPrefab, boardManager.Top);
                 }
           }
                // if up... MUST always be in bottom
           if (direction.x == -1 && boardManager.AllSlots[row, 1].Card != null && playerManager.myLocation == location.bottom)
           {
                Debug.Log("moveoutofendZone From Bottom, pressed up");
                playerManager.spawnPlayerCardAtSlot(boardManager.AllSlots[row, 1], playerManager.PlayerCardPrefab, boardManager.Bottom);
          }
            //down .... MUST alwayzs be in top
            if (direction.x == 1 && boardManager.AllSlots[row, 1].Card != null && playerManager.myLocation == location.top)
            {
                Debug.Log("moveoutofendZone from Top, pressed down");
                playerManager.spawnPlayerCardAtSlot(boardManager.AllSlots[row, 1], playerManager.PlayerCardPrefab, boardManager.Top);
            }

            
        }

        // these moves above can all fail, if they fail their check, then no card is spawned
       
        if (playerManager.myPlayerCard != null)
        {
            playerManager.ActionPoints = 0;
            playerManager.turnsOnBoard = 1;

            playerManager.Doctor.SetActive(false);
            // card was spawned, player has moved out of zone sucessfully,

            // now they need to see if the stole kidney or not
            while (playerManager.myPlayerCard.transform.position != playerManager.mySlot.transform.position) // wait until card finished moving
            {
                yield return null;
            }
        }

       
        isMoving = false; // card finished moving now can take input
        Debug.Log("Done move outof endzone");
    }

    public void isValidMove()
    {
        // need current location, is on board? or is off board

        // is off board
            
    }
    public void MoveLeft()
    {   //
            Vector2 newPos = new Vector2(0, -1);
            Move(newPos,0);
      
    }
    public void MoveRight()
    {

            Vector2 newPos = new Vector2(0, 1);
            Move(newPos,0);

    }
    public void MoveUp()
    {

            Vector2 newPos = new Vector2(-1,0);
            Move(newPos, 0);

    }
    public void MoveDown()
    {   
        //problem if the card doenst exist, it has no arrows to check 
        //should not check here

        // need to check that card exists// move from endzone
        
            Vector2 newPos = new Vector2(1, 0);
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
