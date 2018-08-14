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

    public HPVisual hpvis;
    public APVisual apvis;
    public KidneyVisual kvis;

    public OneCardManager myCardManager;
    public GameObject myPlayerCard;

    public CardAsset playerCardAsset;
    private BoardManager boardManager;
    public HandVisual handvisual;

    public CardAsset[] Deck;

    public GameObject Doctor;
    public GameObject KidneyLocation;

    public Button button;

    public GameObject PortaitGlowObject;
    public GameObject EndTurnGlowObject;

    public Sprite PortraitFull;
    public Sprite PortraitHalf;
    // Kidneys shit 
    public GameObject KidneyPrefab;
    public List<GameObject> patientKidneys = new List<GameObject>(); //kidneys safe in the patient
    public List<GameObject> playerKidneys = new List<GameObject>();  //kidney on board that player has

    // movement points
    // need to know which side is theirs
    // needs to have connections to card Elements?

    
    public location mySide; //top or bottom
    public location myLocation;

    public int MaxAp;
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

            kvis.TurnsTilyouLoose = 3 - turnsWithoutKidney;
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
            if (actionPoints > 4)
                actionPoints = 4;
            apvis.AvailableAp = actionPoints;

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

            if (hp > 15)
                hp = 15;

            hpvis.AvailableHp = hp;

        }
    }
    
    
    public IEnumerator PlaySpellCard(string spell)
    {
        var gm = GameManager.Instance;
        switch (spell)
        {
            case "Swap":
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
            case "Rotate":
                Debug.Log("starting rotate");

                yield return StartCoroutine(gm.selectionManager.getSelection(1));
                if (gm.selectionManager.points.Count == 1)
                {
                    Debug.Log("here");
                    boardManager.RotateArrows(gm.selectionManager.points[0]);
                } else
                {
                    Debug.Log("cancled");
                }
                break;
            case "Replace2":
                yield return StartCoroutine(gm.selectionManager.getSelection(2));
                if (gm.selectionManager.points.Count == 2)
                {
                    boardManager.Replace2(gm.selectionManager.points[0], gm.selectionManager.points[1]);
                } else
                {
                    Debug.Log("cancled");
                }
                break;
            case "Damage":
                yield return StartCoroutine(gm.selectionManager.getSelection(1));
                if (gm.selectionManager.points.Count == 1)
                {
                    OneCardManager damageMeCard = boardManager.FindCardAtPoint(gm.selectionManager.points[0]);

                    if (damageMeCard.cardAsset.Type != CardType.Player)
                    {
                        Debug.Log("start damaging");
                        boardManager.Damage(gm.selectionManager.points[0]);
                    }

                }
                else
                {
                    Debug.Log("cancled");
                }
                break;
            case "Heal":
                Hp += 5;              
                break;
            case "Boost":
                ActionPoints++;
                break;
        }
        Debug.Log("update cards");
        BoardManager.Instance.UpdateCards();
        yield return null;
    }

    public void callSpellCard (string tt)
    {
        Debug.Log("playspellcard");
        StartCoroutine(PlaySpellCard(tt));
        
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
        //AudioManager.instance.Play("dieSound");
        MaxAp = 1;
        apvis.TotalAp = MaxAp;
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

                playerKidneys[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                playerKidneys[i].transform.position = new Vector3(0f, 0f, 0f);

                playerKidneys[i].transform.SetParent(KidneyLocation.transform, true);
                playerKidneys[i].SetActive(false);
                playerKidneys.Remove(playerKidneys[i]);
                Debug.Log("captureKidney");
                kvis.AvailableKidneys++;
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
                //AudioManager.instance.Play("dropKidney");
                //AudioManager.instance.Play("splat");
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
        
        PlayerManager otherPlayer = GameManager.Instance.getOtherPlayer(this);

        if (playerKidneys.Count == 0 && otherPlayer.patientKidneys != null) // && OtherPlayer.patientKidneys != null
        {   
            if (otherPlayer.mySide == myLocation){
                playerKidneys.Add(otherPlayer.patientKidneys[0]); // give player the kidney

                otherPlayer.kvis.AvailableKidneys--;
                otherPlayer.patientKidneys[0].SetActive(true);

                otherPlayer.patientKidneys[0].transform.SetParent(myPlayerCard.transform, false);

                otherPlayer.patientKidneys[0].transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                otherPlayer.patientKidneys[0].transform.localPosition = new Vector3(0f, 0f, 0f);
                otherPlayer.patientKidneys[0].transform.localScale = new Vector3(1f, 1f, 1f);
                Debug.Log("FUCUCUCU");
                
                otherPlayer.patientKidneys.Remove(playerKidneys[0]);
                //AudioManager.instance.Play("stealKidneySound");

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

    public IEnumerator DealPlayerCards(int amount)
    {
        playerInput.InputEnabled = false;

        for (int i = 0; i < 1; i++)
        {
            handvisual.GivePlayerACard(Deck[UnityEngine.Random.Range(0, Deck.Length)], true, true);
            yield return new WaitForSeconds(0.4f);
        }

        playerInput.InputEnabled = true;
        yield return null;
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

            if (c.cardAsset.Type == CardType.Monster)
            {
                MaxAp++;
                apvis.TotalAp = MaxAp;
            }
               
            Debug.Log("add in max AP increase here");
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
        kvis.AvailableKidneys = 1;
        MaxAp = 1;
        apvis.TotalAp = MaxAp;
        ActionPoints = 1;
        
        Hp = 10;
        // for testing // this spawns a player card

        // instantate kideny object and assign add it to the list 
        patientKidneys.Add(Instantiate(KidneyPrefab));
        
        //patientKidneys[0].transform.position = KidneyLocation.transform.position;
        patientKidneys[0].transform.rotation = Quaternion.Euler(90f,0f,0f);//set kidney to proper place and //parent kidney
        patientKidneys[0].SetActive(false);

        if (mySide == location.top)
        {
            Doctor.transform.position = BoardManager.Instance.Top.transform.position;
        } else
        {
            Doctor.transform.position = BoardManager.Instance.Bottom.transform.position;
        }
        
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

        if (playerMover.isMoving || GameManager.Instance.CurrentPlayerTurn != this) // dont allow second move if already moving 
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
        
           

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
