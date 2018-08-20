﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class statusEffect
{
    public statusEffecttype St;
    public int Rounds; //total number of rounds to apply effect
    public int Val;

    public statusEffect(statusEffecttype st, int rounds, int val)
    {
        St = st;
        Rounds = rounds;
        Val = val;
    }

}
public enum statusEffecttype {poisoned,healing,nothing};


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
    private List <statusEffect> statusEffects = new List<statusEffect>();
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

    public List <CardAsset> Deck;

    public GameObject Doctor;
    public GameObject KidneyLocation;

    public Button button;

    public GameObject PortaitGlowObject;
    public GameObject EndTurnGlowObject;
    public GameObject ParticalEff;
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
    private int maxAp;
    public int AvailableAp
    {
        get { return maxAp; }

        set
        {
            if (value <= 4)
            {
                maxAp = value;
            }
        }
    }
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
            if (actionPoints < 0)
                 actionPoints = 0;
           
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
                yield return StartCoroutine(gm.selectionManager.getSelectionWithPlayers(2));

                Debug.Log(gm.selectionManager.points.Count);
                if (gm.selectionManager.points.Count == 2)
                {
                    Debug.Log("swapping");
                    boardManager.SwapCard(gm.selectionManager.points[0], gm.selectionManager.points[1]);
                    BoardManager.Instance.UpdateCards();
                } else
                {
                    Debug.Log("cancled");
                }
                break;
            case "Rotate":
                Debug.Log("starting rotate");

                yield return StartCoroutine(gm.selectionManager.getSelectionWithPlayers(1));
                if (gm.selectionManager.points.Count == 1)
                {
                    Debug.Log("here");
                    boardManager.RotateArrows(gm.selectionManager.points[0]);
                    BoardManager.Instance.UpdateCards();
                } else
                {
                    Debug.Log("cancled");
                }
                break;
            case "Replace2":
                yield return StartCoroutine(gm.selectionManager.getSelectionNoPlayers(2));
                if (gm.selectionManager.points.Count == 2)
                {
                    boardManager.Replace2(gm.selectionManager.points[0], gm.selectionManager.points[1]);
                    BoardManager.Instance.UpdateCards();
                } else
                {
                    Debug.Log("cancled");
                }
                break;
            case "Damage":
                Debug.Log("WTF");
                //GameManager.Instance.getOtherPlayer(this).takeDamage(5) ;

                statusEffect st = new statusEffect(statusEffecttype.poisoned, 3, 3);
                GameManager.Instance.getOtherPlayer(this).statusEffects.Add(st);
                break;
            case "Heal":
                //GameManager.Instance.CurrentPlayerTurn.Hp += 5;
                statusEffect st2 = new statusEffect(statusEffecttype.healing, 2, 3);
                statusEffects.Add(st2);
                break;
            case "Boost":
                ActionPoints++;
                BoardManager.Instance.UpdateCards();
                break;
        }
        Debug.Log("update cards");

        
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.EnableInputs();
    }

    public void clearStatusEffects()
    {
        statusEffects.Clear();
    }

    public void applyStatusEffects()
    {
        cancelOutStatusEffects();

        int damageOrHeal = 0;
        for (int i=0; i< statusEffects.Count; i++) 
        {

            Debug.Log("whatsyourflavorxx");
            statusEffect s = statusEffects[i];
            
            if (s.St == statusEffecttype.healing)
            {    
                damageOrHeal -= s.Val;
            }
            if (s.St == statusEffecttype.poisoned)
            {
                damageOrHeal += s.Val;
            }

            s.Rounds--;

            if (s.Rounds <= 0)
            {
                statusEffects.RemoveAt(i);
                i--;
                continue;
            }
        }

        takeDamage(damageOrHeal);
    }
    public void cancelOutStatusEffects()
    {    
        while (true)
        {
            statusEffect hasHeal = statusEffects.Find(x => x.St == statusEffecttype.healing); 
            statusEffect hasPoison = statusEffects.Find(x => x.St == statusEffecttype.poisoned);

            if (hasHeal != null && hasPoison != null)
            {
                statusEffects.Remove(hasHeal);
                statusEffects.Remove(hasPoison);
                continue;
            }

            break;
        }
        
    }
    public void MoveKidneyFromCardToDoctor()
    {

        if (playerKidneys.Count > 0) // check if kidney on card and if player has room for kidney
        {
            Debug.Log("MoveKidneyFromDoctorToCard");
            playerKidneys[0].transform.SetParent(Doctor.transform, false);
            playerKidneys[0].transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            playerKidneys[0].transform.localPosition = new Vector3(0f, 0f, 0f);
            playerKidneys[0].transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    public void callSpellCard (string tt)
    {
        StartCoroutine(PlaySpellCard(tt));
        
    }
    public void Die()
    {
        Debug.Log("dieing");
        //delete card

        DropKidney(); //drops kidney if possible
        Debug.Log("dropped");

        if (myLocation == location.board)
        myCardManager.CardFaceGlowObject.SetActive(false);

        AvailableAp = 1;
        Debug.Log("actionpoints0");
        ActionPoints = 0;

        apvis.AvailableAp = ActionPoints;
        apvis.TotalAp = AvailableAp;

        Hp = 10;
        if (myLocation == location.board)
        {
            Debug.Log("what");
            if (mySide == location.bottom) // needs to come at end
                StartCoroutine(playerMover.MoveIntoEndzone(boardManager.Bottom));
            else StartCoroutine(playerMover.MoveIntoEndzone(boardManager.Top));
        } else
        {
            Debug.Log("TODOtttt");
            if (mySide == location.bottom) // needs to come at end
                StartCoroutine(playerMover.MoveIntoEndzone(boardManager.Bottom));
            else StartCoroutine(playerMover.MoveIntoEndzone(boardManager.Top));
            // add in fuction that deletes guy from endzone and moves him back to endzone
        }


        clearStatusEffects();
        //AudioManager.instance.Play("dieSound");
 
    }

    public void CaptureKidney()
    {   
        
        int count = playerKidneys.Count;

        if (count > 0)
        {
            Debug.Log("capture kidney");
            patientKidneys.Add(playerKidneys[0]);

            playerKidneys[0].transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            playerKidneys[0].transform.position = new Vector3(0f, 0f, 0f);

            playerKidneys[0].transform.SetParent(KidneyLocation.transform, true);
            playerKidneys[0].SetActive(false);
            playerKidneys.Remove(playerKidneys[0]);
            Debug.Log("captureKidney");
            kvis.AvailableKidneys++;
            if (Hp < 10)
            {
                Hp = 10;
                    
            }               
            
        }
    }

    public void Loose()
    {
        // if turnsWithoutKidney >= 3 then loosegame
    }

    public void DropKidney()
    {
        Debug.Log("dropkidney");
        Debug.Log(myLocation);
        Vector3 pos;
        int count = playerKidneys.Count;

        if (myLocation == location.board)
        {
            CardSlotManager cardslot;
            cardslot = boardManager.FindSlotAtPoint(point);
            

            if (count > 0)
            {
                Debug.Log("removing kidney from player");
                cardslot.Kidneys.Add(playerKidneys[0]);

                cardslot.Kidneys[0].transform.SetParent(cardslot.transform, false);
                pos = cardslot.Kidneys[0].transform.position;


                Sequence mySequence = DOTween.Sequence();

                mySequence.Append(cardslot.Kidneys[0].transform.DOMove(new Vector3(cardslot.transform.position.x, transform.position.y + 6, transform.position.z), 1.2f));
                mySequence.Append(cardslot.Kidneys[0].transform.DOMove(pos, 1.2f));
                //AudioManager.instance.Play("dropKidney");
                //AudioManager.instance.Play("splat");
                DOTween.Play(mySequence);

                playerKidneys.Remove(playerKidneys[0]);
                //mySequence.Insert(0, transform.DORotate(new Vector3(3, 3, 3), mySequence.Duration()));
                //mySequence.Insert(0, transform.DORotate(new Vector3(3, 3, 3), mySequence.Duration()));

                // move towards camera
                // rotate a bit 

                //land on card slot
                // splat

                // mySequence.Append(transform.DOMoveX(45, 1));


            }
        } else // dropping in endzone need to return to patient
        {
            if (count > 0)
            {
                Debug.Log("removing kidney from player");

                if (myLocation == mySide)
                {
                    // can actually never happen because you capture kidney, dont worry about me
                }
                else
                {
                    GameManager.Instance.getOtherPlayer(GameManager.Instance.CurrentPlayerTurn).patientKidneys.Add(playerKidneys[0]);
                    pos = playerKidneys[0].transform.position;

                    Sequence mySequence = DOTween.Sequence();

                    mySequence.Append(playerKidneys[0].transform.DOMove(new Vector3(transform.position.x, transform.position.y + 6, transform.position.z), 1.2f));
                    mySequence.Append(playerKidneys[0].transform.DOMove(pos, 1.2f));
                    //AudioManager.instance.Play("dropKidney");
                    //AudioManager.instance.Play("splat");
                    DOTween.Play(mySequence);


                    playerKidneys[0].transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                    playerKidneys[0].transform.position = new Vector3(0f, 0f, 0f);

                    playerKidneys[0].transform.SetParent(GameManager.Instance.getOtherPlayer(GameManager.Instance.CurrentPlayerTurn).KidneyLocation.transform, true);
                    playerKidneys[0].SetActive(false);
                    playerKidneys.Remove(playerKidneys[0]);

                    GameManager.Instance.getOtherPlayer(this).kvis.AvailableKidneys++;
                }
            }
        }  
    }

    public void PickUpKidneyFromBoard()
    {
        CardSlotManager cardslot;

        cardslot = boardManager.FindSlotAtPoint(point);

        int count = cardslot.Kidneys.Count;

        if (count > 0 && playerKidneys.Count == 0)
        {
            Debug.Log("PickupKidney");
            playerKidneys.Add(cardslot.Kidneys[0]);
            cardslot.Kidneys[0].transform.SetParent(myPlayerCard.transform, false);
            cardslot.Kidneys.Remove(cardslot.Kidneys[0]);
            
        }
    }

    public void StealKidney()
    {
        
        PlayerManager otherPlayer = GameManager.Instance.getOtherPlayer(this);

        if (playerKidneys.Count == 0 && otherPlayer.patientKidneys.Count > 0) // && OtherPlayer.patientKidneys != null
        {   
            if (otherPlayer.mySide == myLocation){
                playerKidneys.Add(otherPlayer.patientKidneys[0]); // give player the kidney

                otherPlayer.kvis.AvailableKidneys--;
                otherPlayer.patientKidneys[0].SetActive(true);

                otherPlayer.patientKidneys[0].transform.SetParent(Doctor.transform, false);

                otherPlayer.patientKidneys[0].transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
                otherPlayer.patientKidneys[0].transform.localPosition = new Vector3(0f, 0f, 0f);
                otherPlayer.patientKidneys[0].transform.localScale = new Vector3(1f, 1f, 1f);
                
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
            playerKidneys[0].transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            playerKidneys[0].transform.localPosition = new Vector3(0f, 0f, 0f);
            playerKidneys[0].transform.localScale = new Vector3(1f, 1f, 1f);
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
            CardAsset cs = Deck[UnityEngine.Random.Range(0, Deck.Count)];

            handvisual.GivePlayerACard(cs, true, true);
            Deck.Remove(cs);
            yield return new WaitForSeconds(0.4f);
        }

        playerInput.InputEnabled = true;
        yield return null;
    }
   

    public void takeDamage(int damage)
    {
        Debug.Log(myLocation);
        if (damage == 0)
        return;

        if (myLocation == location.board)
        {
            DamageEffect.CreateDamageEffect(myPlayerCard, damage);

        } else
        {
            DamageEffect.CreateDamageEffect(Doctor, damage);
        }

        Hp -= damage;

        if (Hp <= 0)
        {
            Die();
        }   
    }

    public void raiseAp()
    {

        Vector3 ApLocation = new Vector3(0, 0, 0);
        Debug.Log("RaisAP max ap is");
        Debug.Log(AvailableAp);
        if (AvailableAp < 4) { 
            ApLocation = apvis.ActionPoints[AvailableAp].transform.position;
        StartCoroutine(AddAvailableApp());
        DamageEffect.CreateMoveEffect(ParticalEff, myPlayerCard.transform.position, ApLocation);
         }
    }


    public IEnumerator AddAvailableApp()
    {
        Debug.Log("ADDING AP!!");
        yield return new WaitForSeconds(1f);
        AvailableAp++;
        apvis.TotalAp = AvailableAp;
        
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
        AvailableAp = 1;
        apvis.TotalAp = AvailableAp;
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
