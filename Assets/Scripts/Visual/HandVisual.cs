using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HandVisual : MonoBehaviour
{
    // PUBLIC FIELDS
    //public AreaPosition owner;

    public PlayerManager p;
    private Canvas canvas;

    public bool TakeCardsOpenly = true;
    public SameDistanceChildren slots;

    [Header("Transform References")]
    public Transform DrawPreviewSpot;
    public Transform DeckTransform;
    public Transform OtherCardDrawSourceTransform;
    public Transform PlayPreviewSpot;

    public float CardPreviewTime = 1;
    public float CardTransitionTime = 1;
    public float CardTransitionTimeFast = 0.5f;

    // PRIVATE : a list of all card visual representations as GameObjects
    private List<GameObject> CardsInHand = new List<GameObject>();

    // ADDING OR REMOVING CARDS FROM HAND
    private void Awake()
    {
    }
    // add a new card GameObject to hand
    public void AddCard(GameObject card)
    {
        // we allways insert a new card as 0th element in CardsInHand List 
        CardsInHand.Insert(0, card);

        // parent this card to our Slots GameObject
        card.transform.SetParent(slots.transform);

        // re-calculate the position of the hand
        PlaceCardsOnNewSlots();
        UpdatePlacementOfSlots();
    }

    // remove a card GameObject from hand
    public void RemoveCard(GameObject card)
    {
        // remove a card from the list
        CardsInHand.Remove(card);

        // re-calculate the position of the hand
        PlaceCardsOnNewSlots();
        UpdatePlacementOfSlots();
    }

    // remove card with a given index from hand
    public void RemoveCardAtIndex(int index)
    {
        CardsInHand.RemoveAt(index);
        // re-calculate the position of the hand
        PlaceCardsOnNewSlots();
        UpdatePlacementOfSlots();
    }

    // get a card GameObject with a given index in hand
    public GameObject GetCardAtIndex(int index)
    {
        return CardsInHand[index];
    }
        
    // MANAGING CARDS AND SLOTS

    // move Slots GameObject according to the number of cards in hand
    void UpdatePlacementOfSlots()
    {
        float posX;
        if (CardsInHand.Count > 0)
            posX = (slots.Children[0].transform.localPosition.x - slots.Children[CardsInHand.Count - 1].transform.localPosition.x) / 2f;
        else
            posX = 0f;

        // tween Slots GameObject to new position in 0.3 seconds
        slots.gameObject.transform.DOLocalMoveX(posX, 0.3f);  
    }

    // shift all cards to their new slots
    void PlaceCardsOnNewSlots()
    {
        foreach (GameObject g in CardsInHand)
        {
            // tween this card to a new Slot
            g.transform.DOLocalMoveX(slots.Children[CardsInHand.IndexOf(g)].transform.localPosition.x, 0.3f);

            // apply correct sorting order and HandSlot value for later 
            SetHandSortingOrder();
        }
    }

    public void BringToFront(GameObject g)
    {
        canvas = g.GetComponentInChildren<Canvas>();

        canvas.sortingOrder = 10;
        canvas.sortingLayerName = "SpellCards"; //todo create another layer
    }

    public void SetHandSortingOrder()
    {
        foreach (GameObject g in CardsInHand)
        {
            canvas = g.GetComponentInChildren<Canvas>();

            canvas.sortingOrder = HandSortingOrder(CardsInHand.IndexOf(g));
            canvas.sortingLayerName = "SpellCards";

        }    
    }

    public int GetIndexOfCard(GameObject g)
    {
        return CardsInHand.IndexOf(g);
    }

    private int HandSortingOrder(int placeInHand)
    {
        return ((placeInHand + 1) * 10);
    }
    // CARD DRAW METHODS

    // creates a card and returns a new card as a GameObject
    GameObject CreateACardAtPosition(CardAsset c, Vector3 position, Vector3 eulerAngles)
    {
        // Instantiate a card depending on its type
        GameObject card;
        OneCardManager manager;
                    // this is a spell: checking for targeted or non-targeted spell
        if (c.Targets == TargetingOptions.NoTarget)
        {
            card = GameObject.Instantiate(GameManager.Instance.SpellCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;

            if (card != null)
            {
                manager = card.GetComponent<OneCardManager>();

                manager.cardAsset = c;
                manager.ReadCardFromAsset();

                return card;
            }
        }

        // WHAT IS WAS BEFOREcard = GameObject.Instantiate(GlobalSettings.Instance.NoTargetSpellCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;
        /*else
        {
            card = GameObject.Instantiate(GlobalSettings.Instance.TargetedSpellCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;
            // pass targeting options to DraggingActions
            DragSpellOnTarget dragSpell = card.GetComponentInChildren<DragSpellOnTarget>();
            dragSpell.Targets = c.Targets;
        }*/



        // apply the look of the card based on the info from CardAsset



        return null;
    }

    // gives player a new card from a given position
    public void GivePlayerACard(CardAsset c, bool fast = false, bool fromDeck = true)
    {
        GameObject card;
        if (fromDeck)
            card = CreateACardAtPosition(c, DeckTransform.position, new Vector3(90, 0, 0f));
        else
            card = CreateACardAtPosition(c, OtherCardDrawSourceTransform.position, new Vector3(90, 0, 0f));

        // pass this card to HandVisual class
        AddCard(card);

        BringToFront(card);
        if (p.mySide == location.top)
        {
            card.tag = "Top";
        } else if (p.mySide == location.bottom) {
            card.tag = "Bottom";
        }
        
        // move card to the hand;
        Sequence s = DOTween.Sequence();
        if (!fast)
        {
            // Debug.Log ("Not fast!!!");
            s.Append(card.transform.DOMove(DrawPreviewSpot.position, CardTransitionTime));
            if (TakeCardsOpenly)
                s.Insert(0f, card.transform.DORotate(new Vector3(90f, 0, 0), CardTransitionTime)); 
            else 
                s.Insert(0f, card.transform.DORotate(new Vector3(90f, 0, 0), CardTransitionTime)); 
            s.AppendInterval(CardPreviewTime);
            // displace the card so that we can select it in the scene easier.
            s.Append(card.transform.DOLocalMove(slots.Children[0].transform.localPosition, CardTransitionTime));
        }
        else
        {
            // displace the card so that we can select it in the scene easier.
            s.Append(card.transform.DOLocalMove(slots.Children[0].transform.localPosition, CardTransitionTimeFast));
            //if (TakeCardsOpenly)    
                //s.Insert(0f,card.transform.DORotate(Vector3.zero, CardTransitionTimeFast)); 
        }

        //s.OnComplete(()=>ChangeLastCardStatusToInHand(card, w));
    }

    // this method will be called when the card arrived to hand 
    void ChangeLastCardStatusToInHand()
    {
        
        SetHandSortingOrder();

    }


    // PLAYING SPELLS

 

    public void PlayASpellFromHand(GameObject CardVisual)
    {
        
        RemoveCard(CardVisual);

        CardVisual.transform.SetParent(null);

        Sequence s = DOTween.Sequence();
        s.Append(CardVisual.transform.DOMove(PlayPreviewSpot.position, 1f));
        //s.Insert(0f, CardVisual.transform.DORotate(new Vector3(0, 0, 0), 1f));
        s.AppendInterval(2f);
        s.OnComplete(()=>
            {
                Debug.Log(CardVisual.GetComponent<OneCardManager>().cardAsset.name);
                p.callSpellCard(CardVisual.GetComponent<OneCardManager>().cardAsset.name);
                // 
                //Command.CommandExecutionComplete();
                
                Destroy(CardVisual);
            });
    }


}
