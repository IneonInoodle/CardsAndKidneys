using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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

    public GameObject FieldCard; // todo change to decks
    public GameObject PlayerCard;

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

        foreach (CardSlotManager s in AllFieldCardsOneDim)
        {
            allSlots[s.indRow, s.indCol] = s;
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
        Destroy(AllSlots[x, y].Card);
        EmptyCardSlots.Add(AllSlots[x, y]);
    }
    
        
  
  
    public IEnumerator GenerateBoard()
    {
        int count = EmptyCardSlots.Count;
        if (count > 0)
        {
            for (int i = EmptyCardSlots.Count - 1; i >= 0; i--)
            {

                // for testing only


                yield return new WaitForSeconds(0.5f);
                EmptyCardSlots[i].Card = Instantiate(FieldCard);
                    // Set Parent slot
                    
                

                EmptyCardSlots[i].Card.GetComponent<OneCardManager>().myCardSlot = EmptyCardSlots[i];
                // have use removeat because "remove" alone doesnt work while iterating through a list with foreach.
                EmptyCardSlots[i].Card.transform.position = InitialFieldCardPos.transform.position;
                EmptyCardSlots[i].Card.transform.DOMove(EmptyCardSlots[i].transform.position, 0.5f);
                EmptyCardSlots.RemoveAt(i);            
            }
            while (EmptyCardSlots.Count > 0)
            {
                yield return null;
            }
        }
    }
    public void DeleteAllCards(){

        for (int i = 0; i < AllSlots.GetLength(0); i++) //2 rows
        {
            for (int j = 0; j < AllSlots.GetLength(1); j++) //3 col
            {

                 //fixes glitched cards

                if (AllSlots[i, j].Card != null)
                {
                    EmptyCardSlots.Add(AllSlots[i, j]);
                    StartCoroutine(AllSlots[i, j].Card.GetComponent<OneCardManager>().DeleteThisCard());                                                  
                }
            }
        }
    } 
    void Update() // all for testing
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //Generate();
            StartCoroutine(GenerateBoard());
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
        foreach (CardSlotManager s in AllSlots)
        {
            if (s.Card != null) { //only check if card there
                if (s.Card.GetComponent<OneCardManager>().cardAsset.Damage != 0) // check if field card
                {
                    s.Card.GetComponent<OneCardManager>().SetAdjencyBonus();
                }
            }
                
        }
    }
    public static readonly Vector2[] directions =
    {
        new Vector2(1,0),  //down
        new Vector2(-1,0), //up
        new Vector2(0,1),  //right
        new Vector2(0,-1)  //left
    };

    public IEnumerator UpdateBoard()
    {
        yield return StartCoroutine(GenerateBoard());
        
        SetAdjencyBonuses();
        yield return new WaitForSeconds(1);
    }
}
