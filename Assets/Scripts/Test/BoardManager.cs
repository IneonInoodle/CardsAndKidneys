using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;
using UnityEngine.Experimental.UIElements;

[System.Serializable]
public struct Point
{
    public int X; //{ get; set; }
    public int Y; //{ get; set; }

    public Point(int p1=0, int p2=0)
    {
        X = p1;
        Y = p2;
    }
}

public class BoardManager : MonoBehaviour {

    private static BoardManager instance;

    public static BoardManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<BoardManager>();
            }
            return instance;
        }
    }


    public GameObject FieldCardPrefab; // todo change to decks

    public CardAsset[] fieldCardAssets;

    public GameObject FieldCardParent; // for Gimbal-Lock, do not fuck with me our else y z rotation stops working

    public GameObject InitialFieldCardPos;
    public GameObject RemoveFieldCardPos;

    public GameObject Top;
    public GameObject Bottom;

    private int rows = 2;
    public int cols = 3;

    private CardSlotManager[,] allSlots = new CardSlotManager[2, 3];
    public CardSlotManager[,] AllSlots { get { return allSlots; } } //no set, set in awake
    public List<CardSlotManager> EmptyCardSlots = new List<CardSlotManager>();

    void Awake()
    {
        instance = this;
        // store card info in allSlots
        CardSlotManager[] AllFieldCardsOneDim = GameObject.FindObjectsOfType<CardSlotManager>();

        foreach (CardSlotManager s in AllFieldCardsOneDim) // converts the 2d array to 3d
        {
            allSlots[s.point.Y, s.point.X] = s; // add in s.point.Y and s.point.X values manually in unity editor
            EmptyCardSlots.Add(s);
        }
    }


    // Use this for initialization
    void Start()
    {


    }



    //var pos1 = (0,0,0);
    //var pos2 = (0, 0, 0);


    public void SwapCard(Point p1, Point p2)
    {
        OneCardManager c1 = FindCardAtPoint(p1);
        OneCardManager c2 = FindCardAtPoint(p2);

        Vector3 v1 = c1.transform.position;
        Vector3 v2 = c2.transform.position;

        c1.transform.DOMove(v2, 0.5f);
        c2.transform.DOMove(v1, 0.5f);

        c1.point = p2;
        c2.point = p1;
    }


    public void Replace2(Point p1, Point p2)
    {
        OneCardManager c1 = FindCardAtPoint(p1);
        OneCardManager c2 = FindCardAtPoint(p2);
        DeleteCard(c1);
        DeleteCard(c2);
        StartCoroutine(DealOutFieldCards(0.5f));
    }

    public Image arrowright;
    public Image arrowleft;
    public Image arrowdown;
    public Image arrowup;

    public void RotateArrows(Point p)
    {
        OneCardManager myCard = FindCardAtPoint(p);
       // if (myCard.gameObject.tag == "Arrows")
        //    Arrow.gameObject.SetActive(false);
        //arrowss = GameObject.FindGameObjectsWithTag("Arrows");
        //foreach (GameObject r in arrowss)
        //   r.transform.Translate(Direction * movespeed * Time.deltaTime);


        
        arrows t = arrows.None;

        if ((myCard.arrows & arrows.Up) == arrows.Up)
        {
            t |= arrows.Down;
        }

        if ((myCard.arrows & arrows.Down) == arrows.Down)
        {
            t |= arrows.Up;
        }

        if ((myCard.arrows & arrows.Right) == arrows.Right)
        {
            t |= arrows.Left;
        }

        if ((myCard.arrows & arrows.Left) == arrows.Left)
        {
            t |= arrows.Right;
        }
        myCard.updateArrows(t);
        
    }

    public CardSlotManager FindSlotAtPoint(Point p)
    {
        return AllSlots[p.Y, p.X];
    }
        
    public OneCardManager FindCardAtPoint(Point p)
    {
        OneCardManager[] AllCards = GameObject.FindObjectsOfType<OneCardManager>();

        foreach (OneCardManager card in AllCards)
        {   
            
            if (Point.Equals(card.point, p) && card.cardAsset.Type != CardType.Spell)
            {
                return card;
            }

        }
        return null;
    }
    
    public GameObject CreateCard(Point p, GameObject cardPrefab, GameObject initPosition, CardAsset cardAsset,  float delay)
    {
        GameObject card;
        //card = Instantiate(cardPrefab);

        ; //keep it bewtween 2 and 1

        card = Instantiate(cardPrefab, initPosition.transform.position, Quaternion.Euler(0, 0, 0 + UnityEngine.Random.Range(-2, 2)));

        card.transform.SetParent(FieldCardParent.transform, false);
        card.GetComponent<OneCardManager>().cardAsset = cardAsset;
        card.GetComponent<OneCardManager>().ReadCardFromAsset();
        card.GetComponent<OneCardManager>().point = p;

        EmptyCardSlots.Remove(AllSlots[p.Y, p.X]); // cannot put it here sadly
        card.transform.position = initPosition.transform.position;
        //Debug.Log("R_"+card.transform.rotation);
        //card.transform.Rotate(new Vector3(1, 2, -1));

        Vector3 v = new Vector3(AllSlots[p.Y, p.X].transform.position.x + UnityEngine.Random.Range(-0.05f, 0.05f), AllSlots[p.Y, p.X].transform.position.y + UnityEngine.Random.Range(-0.05f, 0.05f), AllSlots[p.Y, p.X].transform.position.z);
        card.transform.DOMove(v, delay); //AllSlots[p.Y, p.X].transform.position
        return card;
    }

    public void SetCardAsset(OneCardManager card, CardAsset cardAsset)
    {
        card.cardAsset = cardAsset;
    }


    public void RemoveEmptySlot(Point p)
    {
        EmptyCardSlots.Remove(AllSlots[p.Y, p.X]);
    }
    public IEnumerator DealOutFieldCards(float delay)
    {   // itterates through the emptycardslot list and spawns cards at each.
        // coud convert list to array, go through array and delte list.
        GameObject card;
        int count = EmptyCardSlots.Count;

        if (count > 0)
        {
            for (int i = EmptyCardSlots.Count-1; i >= 0; i--)
            {
                SoundManager.PlaySound("dealCardSound");
                Vector2 newPos = EmptyCardSlots[i].transform.position;
                card = CreateCard(EmptyCardSlots[i].point, FieldCardPrefab, InitialFieldCardPos, fieldCardAssets[UnityEngine.Random.Range(0, fieldCardAssets.Length)], delay);
                 
                //maybe add some stuff to card deal in animaton
                   
                

                //EmptyCardSlots.Remove(EmptyCardSlots[i]);
                yield return new WaitForSeconds(delay);
            }
        }
        yield return null;
    }
    public void DeleteAllCards(){
        OneCardManager[] AllCards = GameObject.FindObjectsOfType<OneCardManager>();

        foreach (OneCardManager card in AllCards)
        {
            DeleteCard(card);
        }

    } 
    public void DeleteCard(OneCardManager card)
    {
        //Debug.Log(card.point.X + " " + card.point.Y);
        AddEmptySlot(card.point);
        StartCoroutine(card.DeleteThisCard());
    }

    public void AddEmptySlot(Point p)
    {
        EmptyCardSlots.Add(AllSlots[p.Y, p.X]);
    }

    public List<OneCardManager> GetNeighborCards(OneCardManager card)
    {
        List<OneCardManager> nList = new List<OneCardManager>();

        if (card != null)
        {
            OneCardManager neighbor;

            foreach (Vector2 dir in directions)
            {
                Point p = new Point(card.point.X + (int)dir.x, card.point.Y + (int)dir.y);
                
                if (FindCardAtPoint(p) != null)
                {
                    neighbor = FindCardAtPoint(p);
                    nList.Add(neighbor);
                }
  
            }
        }
        return nList;
    }

    public int getAdjencyBonus(OneCardManager card)
    {   
        List<OneCardManager> NeigborCards = GetNeighborCards(card);
        int bonus = 0;

        foreach (OneCardManager neigbor in NeigborCards)
        {
            
            if (neigbor.cardAsset.Damage != 0) // check if field card
            {
                bonus += 1;
            }
             
        }
        return bonus;
    }
    void Update() // all for testing
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //Generate();
            StartCoroutine(DealOutFieldCards(0.2f));
        }
        if (Input.GetKeyDown(KeyCode.I))
        {

            SwapCard(new Point(1, 1), new Point(0, 0));
            //Generate();
            //StartCoroutine(GenerateTest());
        }
        

        if (Input.GetKeyDown(KeyCode.Y))
        {
            //Generate();
            
       DeleteAllCards();
                    

            
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            //For Swapping();
            //last_fire_time = Time.time;
            



        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            SetAdjencyBonuses();
        }
        
    }

    public void SetAdjencyBonuses()
    {
        OneCardManager[] AllCards = GameObject.FindObjectsOfType<OneCardManager>();

        foreach (OneCardManager card in AllCards)
        {
            if (card.cardAsset.Type == CardType.Monster) // check if field card
            {
                card.AdjencyBonus = getAdjencyBonus(card);
            }
        }
    }

    public IEnumerator UpdateBoard()
    {
        yield return new WaitForSeconds(1f); // very strage behavor if I delete this
        yield return StartCoroutine(DealOutFieldCards(0.2f));
        
        SetAdjencyBonuses();

        // todo set glows
        yield return new WaitForSeconds(1);
    }

    public static readonly Vector2[] directions =
    {
        new Vector2(1,0),  //down
        new Vector2(-1,0), //up
        new Vector2(0,1),  //right
        new Vector2(0,-1)  //left
    };
}
