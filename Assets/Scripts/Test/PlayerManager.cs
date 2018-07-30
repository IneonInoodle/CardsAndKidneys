using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public enum location { top, bottom, board };

[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerManager : TurnManager {

    public PlayerMover playerMover;
    public PlayerInput playerInput;

    public Text HpText;
    public Text ActionPointsText;

    

    public CardSlotManager mySlot;
    public OneCardManager myCardManager;
    public GameObject myPlayerCard;
    public GameObject PlayerCardPrefab;
    private BoardManager boardManager;

    public GameObject Doctor;
    public GameObject Patient;

    // Kidneys shit 
    public GameObject KidneyPrefab;
    public List<GameObject> patientKidneys = new List<GameObject>(); //kidneys safe in the patient
    public List<GameObject> playerKidneys = new List<GameObject>();  //kidney on board that player has

    // movement points
    // need to know which side is theirs
    // needs to have connections to card Elements?

    
    public location mySide; //top or bottom
    public location myLocation;

    public int turnsOnBoard;

    public UnityEvent finishTurnEvent;
    



    public int actionPoints;
    public int ActionPoints
    {
        get
        {
            return actionPoints;
        }
        set
        {
            actionPoints = value;
            ActionPointsText.text = (actionPoints.ToString() + " / " + turnsOnBoard.ToString());
            if (actionPoints == 0)
            {
                // end turn
            }
        }
    }

    private int hp = 30;
    public int Hp
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            HpText.text = hp.ToString();
            if (hp <= 0)
            {
                //die
            }
        }
    }
    public int drawCards;
    
    public void spawnPlayerCardAtSlot(CardSlotManager c, GameObject cardPrefab, GameObject initPos)
    {
        Debug.Log("ffs");
        OneCardManager CardManagerToDelete;
        //TODO Should rename this method to spawn player card at spot 
        // TODO save damage from card here before delet and apply to playercard
        // also needs to set arrows 
        // shares lots in common with move 

        Debug.Log("spawning card at slot " +  c.indCol.ToString() + "  "  + c.indRow.ToString());
        if (c != null) {

            myPlayerCard = Instantiate(cardPrefab);
            myCardManager = myPlayerCard.GetComponent<OneCardManager>(); // save card manager
            myCardManager.myCardSlot = c;   //needed to connect bad programming?

           

            CardManagerToDelete = c.Card.GetComponent<OneCardManager>();

            
            c.Card = myPlayerCard; // card slot need to know what card it now has

            // c.Card = Instantiate(cardPrefab);
            // myPlayerCard = c.Card; //save our card here

            mySlot = c; //save slot of our card
                        //add player to board
                        // spawn card

            myCardManager = myPlayerCard.GetComponent<OneCardManager>(); // save card manager
            myCardManager.myCardSlot = c;   //needed to connect bad programming?

            myPlayerCard.transform.position = initPos.transform.position; 
            myPlayerCard.transform.DOMove(c.transform.position, 1f); //set to correct position

            boardManager.EmptyCardSlots.Remove(mySlot);
            boardManager.EmptyCardSlots.Remove(mySlot); // remove empty list from list

            if (myLocation != mySide)
            {
                Debug.Log("stealkidney");
            }

            myLocation = location.board; // we spawned player card in, so we can be certain it is on the board


            playerMover.setPlayerArrows(CardManagerToDelete);

            PickUpKidneyFromBoard();
            takeDamage((CardManagerToDelete));  //take damage from field card

            turnsOnBoard = 1;
            ActionPoints = 0;
            

            StartCoroutine(CardManagerToDelete.DeleteThisCard()); // Delete Field card
    
        }  
    }

    public void Die()
    {
        Debug.Log("dieing");
        //delete card

        DropKidney(); //drops kidney if possible

        if (mySide == location.bottom)
        StartCoroutine(playerMover.MoveIntoEnzone(boardManager.Bottom));
        else StartCoroutine(playerMover.MoveIntoEnzone(boardManager.Top));


        turnsOnBoard = 1;
        ActionPoints = 0;
        Hp = 0;



        //drop kidney on slot 
        //reset action points
        // respawn in endzone in endzone
    }

    public void StealKidney()
    {
        //check if inside of enemy endzone and enemy has kidney
        // check if have room for kidney

        // player needs to know if it has kidney or not
    }

    public void Loose()
    {
        // if turnsWithoutKidney >= 3 then loosegame
    }

    public void DropKidney()
    {
        
        if (playerKidneys.Count > 0)
        {
            Debug.Log("Dropping Kidney");
            //player has a kidney? //the kidney now needs to be dropped onto the card slot 
            mySlot.Kidneys.Add(playerKidneys[0]); //get first kidney in list

            // kid.SetParent(b,WorldPositionStayTheSame);

            //this should move the kidney to be parented to the cardslot
            playerKidneys[0].transform.SetParent(mySlot.transform, false);
            playerKidneys.Remove(playerKidneys[0]); // remove kidney from player
        }
        //add kidney to cardslot
        // remove kidney from player 

    }

    public void PickUpKidneyFromBoard()
    {
        
        if (mySlot.Kidneys.Count > 0 && playerKidneys.Count == 0) // check if kidney on card and if player has room for kidney
        {
            Debug.Log("pickupKidney");
            playerKidneys.Add(mySlot.Kidneys[0]); // give player the kidney
            
            mySlot.Kidneys[0].transform.SetParent(myPlayerCard.transform, false); //set kidney to be parentet to playercard
            mySlot.Kidneys.Remove(mySlot.Kidneys[0]); // remove kidney from cardslot
        }
        //add kidney to playeer
        //remove kidney from gameboard
        //
    }

    public void PickUpKidneyFromEnemy()
    {
        if (playerKidneys.Count == 0) // && OtherPlayer.patientKidneys != null
        {   
            /*
            playerKidneys.Add(OtherPlayer.patientKidneys[0]); // give player the kidney

            OtherPlayer.patientKidneys[0].transform.SetParent(myPlayerCard.transform, false); //set kidney to be parentet to playercard
            mySlot.Kidneys.Remove(mySlot.Kidneys[0]); // remove kidney from cardslot
            OtherPlayer.patientKidneys.Remove(OtherPlayer.patientKidneys[0]);
            */ 
        }
    }

    public void MoveKidneyFromDoctorToCard()
    {
        
        if (playerKidneys.Count > 0) // check if kidney on card and if player has room for kidney
        {
            Debug.Log("MoveKidneyFromDoctorToCard");
            playerKidneys[0].transform.SetParent(myPlayerCard.transform, false);
        }
        //add kidney to playeer
        //remove kidney from gameboard
        //
    }

    public void MoveKidneyFromCardToDoctor()
    {
        
        if (playerKidneys.Count > 0) // check if kidney on card and if player has room for kidney
        {
            Debug.Log("Move Kidney from Card to Doctor");
            playerKidneys[0].transform.SetParent(Doctor.transform, false);
        }
        //add kidney to playeer
        //remove kidney from gameboard
        //
    }


    public void takeDamage(OneCardManager c)
    {   
        if (c != null)
        {
            int damage = int.Parse(c.DamageText.text);
            Debug.Log(damage);
           Hp -= damage;

            if (Hp <= 0) Die();
        }
    }


    protected override void Awake()
    {
        base.Awake();
        boardManager = BoardManager.Instance;

        turnsOnBoard = 1;
        ActionPoints = 1;
        
        Hp = 20;
        // for testing // this spawns a player card

        // instantate kideny object and assign add it to the list 
        patientKidneys.Add(Instantiate(KidneyPrefab));

        patientKidneys[0].transform.SetParent(Patient.transform, false);  //set kidney to proper place and //parent kidney
        Doctor.SetActive(true);
        

        if (mySide == location.bottom)
        {
            myLocation = location.bottom;
        } else // top player
        {
            myLocation = location.top;
        }
        

        //for testing 
        /*playerKidneys.Add(Instantiate(KidneyPrefab));
        playerKidneys[0].transform.SetParent(Doctor.transform, false);*/

         // hide doctor for testing TODO
        
        boardManager = BoardManager.Instance;
        //playerInput.InputEnabled = true;
        playerMover = GetComponent<PlayerMover>();
        playerInput = GetComponent<PlayerInput>();
    }
    // Use this for initialization
    void Start () {

        //myLocation = location.bottom;
        // turn on doctor object
        //Doctor.enabled = true;
        // set enum location to bottom

    }



    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("kidney");
            //Generate();

            Die();


        }


        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("test");
            //Generate();

           


        }

        if (playerMover.isMoving || gameManager.CurrentPlayerTurn !=  this) // dont allow second move if already moving 
        {
            //Debug.Log("returning");
            return;
        }

        playerInput.GetKeyInput(); // get input
       
        // 2 loops so diagonals not possible
        if (playerInput.V == 0)
        {
            
            if (playerInput.H < 0)
            {
                playerMover.MoveLeft();
            } else if (playerInput.H > 0){
                playerMover.MoveRight();
            }
        }
        if (playerInput.H == 0)
        {
            
            if (playerInput.V < 0)
            {   // if myslot.y != 0 // move into endyone
                playerMover.MoveDown();
            }
            else if (playerInput.V > 0)
            {
                playerMover.MoveUp();
                // if myslot.y != 1 // move into endyone
                
            }
        }
    }
}
