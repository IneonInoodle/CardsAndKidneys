using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System;



public enum location { top, bottom, board };

[Flags]
public enum arrows
{
    None = 0,
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3
}



//
//            NOTES
//
/*
// same combinations
UpandDown = Up | Down,
 OneTwoAndThree = One | Two | Three,
 */
/*[Flags]
public enum arrows
{
    up = 1,
    down = 2,
    left = 4,
    right = 8
}*/

[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerManager : TurnManager {

    public arrows arrows = arrows.None;

    public PlayerMover playerMover;
    public PlayerInput playerInput;

    public Text HpText;
    public Text ActionPointsText;
    public Text TurnsTilYouLooseText;

    public OneCardManager myCardManager;
    public GameObject myPlayerCard;

    public CardAsset playerCardAsset;
    private BoardManager boardManager;

    public GameObject Doctor;
    public GameObject Patient;

    public Button button;

    private GameMangerKelton gm;
    public GameObject PortaitGlowObject;
    public GameObject EndTurnGlowObject;
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
    public Point point;
    public UnityEvent finishTurnEvent;

    private int turnsWithoutKidney;
    public int TurnsWithoutKidney
    {
        get
        {
            return turnsWithoutKidney;
        }
        set
        {
            turnsWithoutKidney = value;

            TurnsTilYouLooseText.text = ((2 - turnsWithoutKidney).ToString() + " /2");
        }
    }

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
                if (myLocation == location.board)
                    myCardManager.CardFaceGlowObject.SetActive(false);
                this.EndTurnGlowObject.SetActive(true);
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
    
    
    public IEnumerator PlaySpellCard(string spell)
    {
        switch (spell)
        {
            case "swap":
                Debug.Log("starting swap");
                yield return StartCoroutine(gm.selectionManager.getSelection(2));

                Debug.Log(gm.selectionManager.points.Count);
                if (gm.selectionManager.points.Count == 2)
                {
                    Debug.Log("swapping");
                    boardManager.SwapCard(gm.selectionManager.points[0], gm.selectionManager.points[1]);
                } else
                {
                    Debug.Log("cancled");
                }
                break;
            case "rotate":
                yield return StartCoroutine(gm.selectionManager.getSelection(1));
                if (gm.selectionManager.points.Count == 1)
                {
                    boardManager.RotateArrows(gm.selectionManager.points[0]);
                } else
                {
                    Debug.Log("cancled");
                }
                break;
            case "replace2":
                yield return StartCoroutine(gm.selectionManager.getSelection(2));
                if (gm.selectionManager.points.Count == 2)
                {

                } else
                {
                    Debug.Log("cancled");
                }
                break;
            case "boost":
                ActionPoints++;
                break;
        }
        yield return null;
    }

    public void test (string tt)
    {
        Debug.Log("playspellcard");
        StartCoroutine(PlaySpellCard("swap"));
        
    }
    public void Die()
    {
        Debug.Log("dieing");
        //delete card

        DropKidney(); //drops kidney if possible

        myCardManager.CardFaceGlowObject.SetActive(false);
        if (mySide == location.bottom)

        StartCoroutine(playerMover.MoveIntoEndzone(boardManager.Bottom));
        else StartCoroutine(playerMover.MoveIntoEndzone(boardManager.Top));

        SoundManager.PlaySound("dieSound");
        turnsOnBoard = 1;
        ActionPoints = 0;
        Hp = 0;



        //drop kidney on slot 
        //reset action points
        // respawn in endzone in endzone
    }

    public void CaptureKidney()
    {
        int count = playerKidneys.Count;

        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                patientKidneys.Add(playerKidneys[i]);
                playerKidneys[i].transform.SetParent(Patient.transform, false);
                playerKidneys.Remove(playerKidneys[i]);
                TurnsWithoutKidney = 0;
                if (Hp < 10)
                {
                    Hp = 10;
                    
                }               
            }
        }
    }

    public void Loose()
    {
        // if turnsWithoutKidney >= 3 then loosegame
    }

    public void DropKidney()
    {
        Vector3 pos;
        CardSlotManager cardslot;
        cardslot = boardManager.FindSlotAtPoint(point);
        int count = playerKidneys.Count;

        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {   
                cardslot.Kidneys.Add(playerKidneys[i]);
                playerKidneys[i].transform.SetParent(cardslot.transform, false);
                pos = playerKidneys[i].transform.position;


                 Sequence mySequence = DOTween.Sequence();

                 mySequence.Append(playerKidneys[i].transform.DOMove(new Vector3(cardslot.transform.position.x, transform.position.y + 6, transform.position.z), 1.2f));
                 mySequence.Append(playerKidneys[i].transform.DOMove(pos, 1.2f));

                SoundManager.PlaySound("dropKidney");
                SoundManager.PlaySound("splat");
                DOTween.Play(mySequence);

                playerKidneys.Remove(playerKidneys[i]);
                //mySequence.Insert(0, transform.DORotate(new Vector3(3, 3, 3), mySequence.Duration()));
                //mySequence.Insert(0, transform.DORotate(new Vector3(3, 3, 3), mySequence.Duration()));

                // move towards camera
                // rotate a bit 

                //land on card slot
                // splat

                // mySequence.Append(transform.DOMoveX(45, 1));


            }
        }
 
    }

    public void PickUpKidneyFromBoard()
    {
        Debug.Log("damn");
        CardSlotManager cardslot;

        cardslot = boardManager.FindSlotAtPoint(point);

        int count = cardslot.Kidneys.Count;

        if (count > 0 && playerKidneys.Count == 0)
        {
            Debug.Log("damn");
            for (int i = 0; i < count; i++)
            {
                Debug.Log("damn");
                playerKidneys.Add(cardslot.Kidneys[i]);
                cardslot.Kidneys[i].transform.SetParent(myPlayerCard.transform, false);
                cardslot.Kidneys.Remove(cardslot.Kidneys[i]);
            }
        }
    }

    public void StealKidney()
    {
        
        PlayerManager otherPlayer = gm.getOtherPlayer(this);

        if (playerKidneys.Count == 0 && otherPlayer.patientKidneys != null) // && OtherPlayer.patientKidneys != null
        {   
            if (otherPlayer.mySide == myLocation){
                playerKidneys.Add(otherPlayer.patientKidneys[0]); // give player the kidney
                otherPlayer.patientKidneys[0].transform.SetParent(myPlayerCard.transform, false); //set kidney to be parentet to playercard
                
                otherPlayer.patientKidneys.Remove(playerKidneys[0]);
                SoundManager.PlaySound("stealKidneySound");
            }
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




    public void takeDamage(OneCardManager c)
    {

       
        if (c != null)
        {
            int damage = int.Parse(c.DamageText.text);
            

            if (c.cardAsset.Type == CardType.Neutral)
            {

            } else if (myLocation == location.board)
            {
                DamageEffect.CreateDamageEffect(myPlayerCard, damage);

            } else
            {
                DamageEffect.CreateDamageEffect(Doctor, damage);
            }
            
            Debug.Log(damage);
            
            Hp -= damage;

            if (Hp <= 0) Die();
        }
    }

    


    /*public void TakeDamage(int amount, int healthAfter)
    {
        if (amount > 0)
        {
            Hp -= amount;
            //DamageEffect.CreateDamageEffect(transform.position, amount);

            if (Hp <= 0) Die();
        }
    }*/

    protected override void Awake()
    {
        base.Awake();
        boardManager = BoardManager.Instance;
        gm = UnityEngine.Object.FindObjectOfType<GameMangerKelton>().GetComponent<GameMangerKelton>(); // could use a singleton

        turnsOnBoard = 5;
        ActionPoints = 5;
        
        Hp = 20;
        // for testing // this spawns a player card

        // instantate kideny object and assign add it to the list 
        patientKidneys.Add(Instantiate(KidneyPrefab));

        patientKidneys[0].transform.SetParent(Patient.transform, false);  //set kidney to proper place and //parent kidney
        Doctor.SetActive(true);

        if (mySide == location.bottom)
        {
            myLocation = location.bottom;
            point.X = 1;
            point.Y = 2;
            arrows = arrows.Up | arrows.Left | arrows.Right;
   
        } else // top player
        {   
            myLocation = location.top;
            point.X = 1;
            point.Y = -1;
            arrows = arrows.Down | arrows.Left | arrows.Right;
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

        if (playerMover.isMoving || gameManager.CurrentPlayerTurn != this) // dont allow second move if already moving 
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("kidney");
            //Generate();
            Die();
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
