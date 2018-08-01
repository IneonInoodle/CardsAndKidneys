using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;



public struct Point
{   
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int p1, int p2)
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

    public GameObject InitialFieldCardPos;
    public GameObject RemoveFieldCardPos;

    public GameObject Top;
    public GameObject Bottom;

    private int rows = 2;
    public int cols = 3;

    private CardSlotManager[,] allSlots = new CardSlotManager[2, 3];
    public CardSlotManager[,] AllSlots { get {return allSlots; } } //no set, set in awake
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
    
    void RemoveCard(int x, int y)
    {

        // generates field cards!!

        //card.transform.position = cardsInitialPosition;
        //card.transform.DOMove(SlotsForCards[i].position, 0.5f);

        //Sequence s = DOTween.Sequence();
        // 1) raise the pack to opening position
        //s.Append(transform.DOLocalMoveZ(-2f, 0.5f));
        //s.Append(transform.DOShakeRotation(1f, 20f, 20));

        //s.OnComplete(() =>
        //{
        // 2) add glow, particle system

        // 3): 
        //ShopManager.Instance.OpeningArea.ShowPackOpening(transform.position);
        //if (ShopManager.Instance.PacksCreated > 0)
        //ShopManager.Instance.PacksCreated--;
        // 4) destroy this pack in the end of the sequence
        //Destroy(gameObject);
        //});
        //Destroy(AllSlots[x, y].Card);
        //EmptyCardSlots.Add(AllSlots[x, y]);
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
            
            if (Point.Equals(card.point, p))
            {
                return card;
            }

        }
        return null;
    }
    
    public GameObject CreateCard(Point p, GameObject cardPrefab, CardAsset cardAsset, float delay)
    {
        GameObject card;
        card = Instantiate(cardPrefab);
        card.GetComponent<OneCardManager>().cardAsset = cardAsset;
        card.GetComponent<OneCardManager>().ReadCardFromAsset();
        card.GetComponent<OneCardManager>().point = p;

        EmptyCardSlots.Remove(AllSlots[p.Y, p.X]); // cannot put it here sadly
        card.transform.position = InitialFieldCardPos.transform.position;

        card.transform.DOMove(AllSlots[p.Y, p.X].transform.position, delay);

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
                card = CreateCard(EmptyCardSlots[i].point, FieldCardPrefab, fieldCardAssets[Random.Range(0, fieldCardAssets.Length)], delay);
                 
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
            //Generate();
            //StartCoroutine(GenerateTest());
        }
        

        if (Input.GetKeyDown(KeyCode.Y))
        {
            //Generate();
            
       DeleteAllCards();
                    

            
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
            if (card.cardAsset.Damage != 0) // check if field card
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
