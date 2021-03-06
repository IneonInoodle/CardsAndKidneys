﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;
//using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
[System.Serializable]
public struct Point
{
    public int X; //{ get; set; }
    public int Y; //{ get; set; }

    public Point(int p1 = 0, int p2 = 0)
    {
        X = p1;
        Y = p2;
    }
}

public class BoardManager : MonoBehaviour
{

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

    public List<CardAsset> fieldCardAssets;

    public GameObject FieldCardParent; // for Gimbal-Lock, do not fuck with me our else y z rotation stops working

    public GameObject InitialFieldCardPos;
    public GameObject RemoveFieldCardPos;
    public GameObject ChatBot;
    public Text ChatBot2;
    public GameObject Top;
    public GameObject Bottom;
    public GameObject PauseButtonArm;
    public GameObject TopCanv;
    public GameObject BottomCanv;

    public GameObject FieldDeck;

    private int rows = 2;
    public int cols = 3;

    private CardSlotManager[,] allSlots = new CardSlotManager[2, 3];
    public CardSlotManager[,] AllSlots { get { return allSlots; } } //no set, set in awake
    public List<CardSlotManager> EmptyCardSlots = new List<CardSlotManager>();
    public List<OneCardManager> AllCards = new List<OneCardManager>();

    void Awake()
    {
        instance = this;

        if (FieldDeck != null)
        {
            fieldCardAssets.AddRange(FieldDeck.GetComponent<FieldDeck>().fieldDeck);
        }
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
        //AudioManager.instance.Play("GameStart");

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

        if (c1.cardAsset.Type == CardType.Player)
        {
            foreach (PlayerManager p in GameManager.Instance.players)
            {
                if (p.myCardManager == c1)
                {
                    p.point = c1.point;
                }
            }
        }

        if (c2.cardAsset.Type == CardType.Player)
        {
            foreach (PlayerManager p in GameManager.Instance.players)
            {
                if (p.myCardManager == c2)
                {
                    p.point = c2.point;
                }
            }
        }

    }


    public void Replace2(Point p1, Point p2)
    {

        OneCardManager c1 = FindFieldCardAtPoint(p1);
        OneCardManager c2 = FindFieldCardAtPoint(p2);
        DeleteCard(c1);
        DeleteCard(c2);

        CreateCard(p1, FieldCardPrefab, InitialFieldCardPos, fieldCardAssets[UnityEngine.Random.Range(0, fieldCardAssets.Count)], 0.5f); // should remove these from the deck tbh
        CreateCard(p2, FieldCardPrefab, InitialFieldCardPos, fieldCardAssets[UnityEngine.Random.Range(0, fieldCardAssets.Count)], 1f);
    }

    public void Damage(Point p1)
    {

        OneCardManager c = BoardManager.Instance.FindFieldCardAtPoint(p1);
        int i = Int32.Parse(c.DamageText.text);
        i = i - 100;

        if (i <= 0)
        {
            DeleteCard(c);
            //StartCoroutine(damagePart(0.5f, p1));
        }
        else
        {
            c.DamageText.text = i.ToString();
        }
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



        if (myCard.cardAsset.Type == CardType.Player)
        {

            foreach (PlayerManager pl in GameManager.Instance.players)
            {
                if (pl.myCardManager == myCard)
                {
                    Debug.Log("rotateplayerArrows");
                    pl.arrows = t;
                    pl.point = myCard.point;
                }
            }
        }

        myCard.updateArrows(t);
    }

    public CardSlotManager FindSlotAtPoint(Point p)
    {
        Debug.Log(p.X + " " + p.Y);
        return AllSlots[p.Y, p.X];
    }

    public OneCardManager FindFieldCardAtPoint(Point p)
    {
        foreach (OneCardManager card in AllCards)
        {

            if (Point.Equals(card.point, p) && card.cardAsset.Type != CardType.Spell && card.cardAsset.Type != CardType.Player)
            {
                return card;
            }

        }
        return null;
    }

    public OneCardManager FindCardAtPoint(Point p)
    {
        foreach (OneCardManager card in AllCards)
        {

            if (Point.Equals(card.point, p) && card.cardAsset.Type != CardType.Spell)
            {
                return card;
            }

        }
        return null;
    }

    public OneCardManager FindPlayerCardAtPoint(Point p)
    {
        foreach (OneCardManager card in AllCards)
        {

            if (Point.Equals(card.point, p) && card.cardAsset.Type == CardType.Player)
            {
                return card;
            }
        }
        return null;
    }

    public void HighlightArrows() // highlight arrows and save all player moves found 
    {
        foreach (OneCardManager card in AllCards)
        {
            if (card.cardAsset.Type != CardType.Spell)
            {
                arrows t = arrows.None;
                if (FindFieldCardAtPoint(new Point(card.point.X, card.point.Y - 1)) != null) t |= arrows.Up;
                if (FindFieldCardAtPoint(new Point(card.point.X - 1, card.point.Y)) != null) t |= arrows.Left;
                if (FindFieldCardAtPoint(new Point(card.point.X, card.point.Y + 1)) != null) t |= arrows.Down;
                if (FindFieldCardAtPoint(new Point(card.point.X + 1, card.point.Y)) != null) t |= arrows.Right;

                if (card.point.Y == 0) t |= arrows.Up;
                if (card.point.Y == 1) t |= arrows.Down;

                card.updateArrowsGlow(t);
            }



        }


    }

    public bool IsEndzoneValid(Point p)
    {

        arrows check = p.Y == 0 ? arrows.Up : arrows.Down;

        OneCardManager c;

        if (FindFieldCardAtPoint(p) != null)
        {
            c = FindFieldCardAtPoint(p);
        }
        else
        {
            c = FindPlayerCardAtPoint(p);
        }

        if ((c.arrows & check) != 0) return true;
        return false;

        /*for(int x=0; x<3;x++)
        {
            OneCardManager card = FindCardAtPoint(new Point(x, y));
            if (card == null) continue;
            if ((card.arrows & check) != 0) return true;
        }*/
    }

    public List<OneCardManager> GetValidMoves(OneCardManager card) // highlight arrows and save all player moves found 
    {
        List<OneCardManager> nList = new List<OneCardManager>();

        if (card != null)
        {
            nList.AddRange(GetValidMoves(card.point, card.arrows));
        }

        return nList;
    }

    public List<OneCardManager> GetValidMoves(Point pos, arrows arr)
    {
        List<OneCardManager> nList = new List<OneCardManager>();

        Point p;

        if ((arr & arrows.Up) != 0)
        {
            p = new Point(pos.X, pos.Y - 1);
            if (FindFieldCardAtPoint(p) != null)
            {
                nList.Add(FindFieldCardAtPoint(p));
            }
        }

        if ((arr & arrows.Down) == arrows.Down)
        {
            p = new Point(pos.X, pos.Y + 1);
            if (FindFieldCardAtPoint(p) != null) // can always move into endzone extra arrows here
            {
                nList.Add(FindFieldCardAtPoint(p));
            }
        }

        if ((arr & arrows.Right) == arrows.Right)
        {
            p = new Point(pos.X + 1, pos.Y);
            if (FindFieldCardAtPoint(p) != null)
            {
                nList.Add(FindFieldCardAtPoint(p));
            }
        }

        if ((arr & arrows.Left) == arrows.Left)
        {
            p = new Point(pos.X - 1, pos.Y);
            if (FindFieldCardAtPoint(p) != null)
            {
                nList.Add(FindFieldCardAtPoint(p));
            }
        }

        if (pos.Y == 2)
        {
            foreach (OneCardManager card in AllCards)
            {
                if (card.point.Y == 1)
                    nList.Add(card);
            }
        }
        else if (pos.Y == -1)
        {
            foreach (OneCardManager card in AllCards)
            {
                if (card.point.Y == 0)
                    nList.Add(card);
            }
        }
        return nList;
    }

    public List<OneCardManager> GetValidMovesDist(Point pos, arrows arr, int dist)
    {
        if (dist == 0) return new List<OneCardManager>();

        List<OneCardManager> nList = GetValidMoves(pos, arr);
        List<OneCardManager> nList2 = new List<OneCardManager>();
        foreach (OneCardManager card in nList)
        {
            List<OneCardManager> nSub = GetValidMovesDist(card.point, card.arrows, dist - 1);
            foreach (OneCardManager card2 in nSub)
            {
                if (!nList.Contains(card2)) nList2.Add(card2); //TODO fix bug here
            }
        }
        nList.AddRange(nList2);
        return nList;
    }

    public void RotatePlayerPortaitZones()
    {

        TopCanv.transform.DOLocalRotate(new Vector3(0f, 0f, TopCanv.transform.localRotation.eulerAngles.z + 180), 0.25f, RotateMode.FastBeyond360);
        BottomCanv.transform.DOLocalRotate(new Vector3(0f, 0f, BottomCanv.transform.localRotation.eulerAngles.z + 180), 0.25f, RotateMode.FastBeyond360);
        RotatePauseButton();
    }
    public void RotatePauseButton()
    {
        PauseButtonArm.transform.DOLocalRotate(new Vector3(0f, 0f, PauseButtonArm.transform.localRotation.eulerAngles.z + 180), 0.1f, RotateMode.FastBeyond360);
    }

    public void RotateFieldCard(OneCardManager c)
    {
        c.frame.transform.DOLocalRotateQuaternion(c.frame.transform.localRotation * Quaternion.Euler(0, 0, GameManager.Instance.camera.transform.localRotation.eulerAngles.y), 0f);
    }


    public IEnumerator RotateFieldCards()
    {
        foreach (OneCardManager c in AllCards)
        {
            //hide arrows
            c.CardArrowLeft.SetActive(false);
            c.CardArrowRight.SetActive(false);
            c.CardArrowDown.SetActive(false);
            c.CardArrowUp.SetActive(false);
            //c.frame.transform.DOLocalRotate(new Vector3(0f, 0f, c.frame.transform.localRotation.eulerAngles.z + 180), 0.25f, RotateMode.FastBeyond360);
            Debug.Log(GameManager.Instance.camera.transform.rotation.eulerAngles.y);


            Debug.Log(c.frame.transform.localRotation.eulerAngles);

            Sequence mySequence = DOTween.Sequence();

            //c.frame.transform.rotation = 

            c.frame.transform.rotation = Quaternion.Euler(270, 180, GameManager.Instance.camera.transform.rotation.eulerAngles.y);
            mySequence.Append(c.frame.transform.DORotateQuaternion(Quaternion.Euler(270, 180, GameManager.Instance.camera.transform.rotation.eulerAngles.y), 0f));
            //mySequence.Append(c.frame.transform.DORotate(new Vector3(270, 0, 0), 0.0f));
            mySequence.Append(c.frame.transform.DORotateQuaternion(c.frame.transform.rotation * Quaternion.Euler(0, 0, 180), 0.25f));

            //mySequence.Append(c.frame.transform.DORotateQuaternion(Quaternion.Euler(270, 180, GameManager.Instance.camera.transform.rotation.eulerAngles.y), 0f));



            DOTween.Play(mySequence);


            //goal is to get card rotation y to 180 so that its not half rotated

            //RotateFieldCard(c);

        }

        yield return new WaitForSeconds(0.25f);
        foreach (OneCardManager cc in AllCards)
        {
            cc.frame.transform.rotation = Quaternion.Euler(270, 180, GameManager.Instance.camera.transform.rotation.eulerAngles.y);
            cc.updateArrows(cc.arrows);
        }
    }


    public OneCardManager CreateCard(Point p, GameObject cardPrefab, GameObject initPosition, CardAsset cardAsset, float delay)
    {
        //card = Instantiate(cardPrefab);

        //keep it bewtween 2 and 1

        OneCardManager card = Instantiate(cardPrefab, initPosition.transform.position, Quaternion.Euler(0, 180, UnityEngine.Random.Range(-1, 1))).GetComponent<OneCardManager>();
        RotateFieldCard(card);
        AllCards.Add(card);

        card.transform.SetParent(FieldCardParent.transform, false);
        card.cardAsset = cardAsset;
        card.ReadCardFromAsset();
        card.point = p;

        EmptyCardSlots.Remove(AllSlots[p.Y, p.X]); // cannot put it here sadly
        card.transform.position = initPosition.transform.position;
        //Debug.Log("R_"+card.transform.rotation);
        //card.transform.Rotate(new Vector3(1, 2, -1));

        Vector3 v = new Vector3(AllSlots[p.Y, p.X].transform.position.x + UnityEngine.Random.Range(-0.05f, 0.05f), AllSlots[p.Y, p.X].transform.position.y + UnityEngine.Random.Range(-0.05f, 0.05f), AllSlots[p.Y, p.X].transform.position.z);
        if (card.cardAsset.Type == CardType.Player)
        {
            v.y = 0.35f;
        }
        card.transform.DOMove(v, delay); //AllSlots[p.Y, p.X].transform.position
        return card;
    }

    public void SetCardAsset(OneCardManager card, CardAsset cardAsset)
    {
        ChatBot2.text = ChatBot2.text + "\n" + System.DateTime.Now.ToString("hh:mm:ss") + ": SetCardAsset()";
        card.cardAsset = cardAsset;
    }


    public void RemoveEmptySlot(Point p)
    {
        Debug.Log(p.X + " " + p.Y);


        ChatBot2.text = ChatBot2.text + "\n" + System.DateTime.Now.ToString("hh:mm:ss") + ": RemoveEmptySlot()";
        EmptyCardSlots.Remove(AllSlots[p.Y, p.X]);
    }
    public IEnumerator DealOutFieldCards(float delay)
    {   // itterates through the emptycardslot list and spawns cards at each.
        // coud convert list to array, go through array and delte list.

        OneCardManager card;
        int count = EmptyCardSlots.Count;
        if (fieldCardAssets.Count <= 6)
        {
            fieldCardAssets.AddRange(FieldDeck.GetComponent<FieldDeck>().fieldDeck);
        }


        ChatBot2.text = ChatBot2.text + "\n" + System.DateTime.Now.ToString("hh:mm:ss") + ": DealOutFieldCards()";
        if (count > 0)
        {
            for (int i = EmptyCardSlots.Count - 1; i >= 0; i--)
            {
                AudioManager.instance.PlaySound("DealCard");
                //FindObjectOfType<AudioManager>().Play("cardPlace1");

                Vector2 newPos = EmptyCardSlots[i].transform.position;
                CardAsset cs = fieldCardAssets[UnityEngine.Random.Range(0, fieldCardAssets.Count)];

                card = CreateCard(EmptyCardSlots[i].point, FieldCardPrefab, InitialFieldCardPos, cs, delay);
                fieldCardAssets.Remove(cs);

                //maybe add some stuff to card deal in animaton



                //EmptyCardSlots.Remove(EmptyCardSlots[i]);
                yield return new WaitForSeconds(delay);
            }
        }
        yield return null;
    }
    public void DeleteAllCards()
    {
        ChatBot2.text = ChatBot2.text + "\n" + System.DateTime.Now.ToString("hh:mm:ss") + ": : DeleteAllCards()";
        while (AllCards.Count > 0)
        {
            DeleteCard(AllCards[0]);
        }

    }
    public void DeleteCard(OneCardManager card)
    {
        Debug.Log("DeletingCard");
        ChatBot2.text = ChatBot2.text + "\n" + System.DateTime.Now.ToString("hh:mm:ss") + ": : DeleteCard()";
        Debug.Log(card.point.X);
        Debug.Log(card.point.Y);
        AddEmptySlot(card.point);
        StartCoroutine(card.DeleteThisCard());
    }

    public void AddEmptySlot(Point p)
    {
        ChatBot2.text = ChatBot2.text + "\n" + System.DateTime.Now.ToString("hh:mm:ss") + ": : AddEmptySlot()";
        Debug.Log(AllSlots[p.Y, p.X]);
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

                if (FindFieldCardAtPoint(p) != null)
                {
                    neighbor = FindFieldCardAtPoint(p);
                    nList.Add(neighbor);
                }

            }
        }
        return nList;
    }

    public int getAdjencyBonus(OneCardManager card)
    {
        ChatBot2.text = ChatBot2.text + "\n" + System.DateTime.Now.ToString("hh:mm:ss") + ": : getAdjencyBonus()";
        List<OneCardManager> NeigborCards = GetNeighborCards(card);
        int bonus = 0;

        foreach (OneCardManager neigbor in NeigborCards)
        {

            if (neigbor.cardAsset.Type == CardType.Monster) // check if field card
            {
                bonus += 1;
            }

        }
        return bonus;
    }
    void Update() // all for testing
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChatBot2.text = ChatBot2.text + "\n" + System.DateTime.Now.ToString("hh:mm:ss") + ": ";
            ChatBot2.text = "Starting on: " + System.DateTime.Now.ToString("hh:mm:ss");
            ChatBot.SetActive(true);

        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            //Generate();
            StartCoroutine(DealOutFieldCards(0.2f));
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
            UpdateCards();
        }

    }

    public void HighlightPaths(OneCardManager card)
    {
        /*
        var allWays = new List<List<OneCardManager>>();

        List<OneCardManager> SelectableCards1 = new List<OneCardManager>();
        List<OneCardManager> SelectableCards2 = new List<OneCardManager>();
        List<OneCardManager> SelectableCards3 = new List<OneCardManager>();
        List<OneCardManager> SelectableCards4 = new List<OneCardManager>();

        SelectableCards1 = HighlightArrows(card); // 1 AP
        foreach (OneCardManager s in SelectableCards1)
        {
            allWays.Add(new List<OneCardManager> {card});
        }

        if (GameManager.Instance.CurrentPlayerTurn.ActionPoints >= 2)
        {
            foreach (OneCardManager ss in SelectableCards1) //2 AP
            {
                SelectableCards2 = HighlightArrows(ss);
                allWays.Add(new List<OneCardManager> { card, ss});


                if (GameManager.Instance.CurrentPlayerTurn.ActionPoints >= 3)
                {
                    foreach (OneCardManager sss in SelectableCards2) //3 AP
                    {
                        SelectableCards3 = HighlightArrows(sss);
                        allWays.Add(new List<OneCardManager> { card, ss, sss});

                        if (GameManager.Instance.CurrentPlayerTurn.ActionPoints >= 3)
                        {
                            foreach (OneCardManager ssss in SelectableCards3) //4 AP
                            {
                                SelectableCards4 = HighlightArrows(ssss);
                                allWays.Add(new List<OneCardManager> { card, ss, sss,ssss});

                            }
                        } 
                    }
                }
            }
        }

        Debug.Log(allWays);
        foreach (List<OneCardManager> l in allWays)
        {
            Debug.Log(l.Point.X);
        }       
        */
    }

    public void UpdateCards() // responsible for adjecny bonus and arrow glow
    {
        Debug.Log("updateCards");
        ChatBot2.text = ChatBot2.text + "\n" + System.DateTime.Now.ToString("hh:mm:ss") + ": : UpdateCards()";
        List<OneCardManager> SelectableCards = new List<OneCardManager>();
        List<OneCardManager> HighlightableCards = new List<OneCardManager>();
        List<OneCardManager>[] a = new List<OneCardManager>[100];

        foreach (OneCardManager card in AllCards)
        {
            card.CardFaceInnerGlowImage.enabled = false;
        }

        foreach (OneCardManager card in AllCards)
        {
            if (card.cardAsset.Type == CardType.Monster) // check if field card
            {
                card.AdjencyBonus = getAdjencyBonus(card);

            }
            if (card.cardAsset.Type != CardType.Spell) // check if field card
            {
                card.gameObject.GetComponent<Selector>().isSelectable = false;
            }

        }

        SelectableCards = GetValidMoves(GameManager.Instance.CurrentPlayerTurn.point, GameManager.Instance.CurrentPlayerTurn.arrows);
        if (SelectableCards.Count == 0)
            GameManager.Instance.CurrentPlayerTurn.button.interactable = true;
        HighlightableCards = GetValidMovesDist(GameManager.Instance.CurrentPlayerTurn.point, GameManager.Instance.CurrentPlayerTurn.arrows, GameManager.Instance.CurrentPlayerTurn.ActionPoints);

        if (GameManager.Instance.CurrentPlayerTurn.ActionPoints == 0)
        {
            SelectableCards.Clear();
        }

        // bring forward all selectable moves
        // if not already brought forward 
            //bring forward
            //fade grey
        // push back all non selectable moves
            // if brought forward, push back and grey out

        foreach (OneCardManager c in SelectableCards)
        {
            Selector s;
            if (c.cardAsset.Type == CardType.Hp ||
                c.cardAsset.Type == CardType.Monster ||
                c.cardAsset.Type == CardType.Neutral)
            {
                c.CardFaceInnerGlowImage.enabled = true;
                s = c.gameObject.GetComponent<Selector>();
                s.isSelectable = true;
            }
        }


        /* Do I really want this in game?
        foreach (OneCardManager c in HighlightableCards)
        {
            if (c.cardAsset.Type == CardType.Hp ||
                c.cardAsset.Type == CardType.Monster ||
                c.cardAsset.Type == CardType.Neutral)
            {
                c.CardFaceInnerGlowImage.enabled = true;
            }
        }*/

        HighlightArrows();
        HighlightEndZones();


        Top.GetComponent<EndzoneManager>().setEndZoneArrows();
        Bottom.GetComponent<EndzoneManager>().setEndZoneArrows();

        BringCardsForwardOrBack();

    }
    // Bring Cards Forward and ungreys them out.
    public void BringCardsForwardOrBack()
    {
        foreach (OneCardManager card in AllCards)
        {
            if (card.CardFaceInnerGlowImage.enabled == true)
            {
                card.transform.DOMoveY(0.5f, 1f);
                card.CardGreyOutImage.enabled = false;
            }   else
            {
                card.transform.DOMoveY(0.1f, 1f);
                card.CardGreyOutImage.enabled = true;
            }
        }

        if (GameManager.Instance.CurrentPlayerTurn.myCardManager != null)
        {
            GameManager.Instance.CurrentPlayerTurn.myPlayerCard.transform.DOMoveY(0.5f, 1f);
            GameManager.Instance.CurrentPlayerTurn.myCardManager.CardGreyOutImage.enabled = false;
        }
    }

    public void HighlightEndZones()
    {
        ChatBot2.text = ChatBot2.text + "\n" + System.DateTime.Now.ToString("hh:mm:ss") + ": : HighlightEndZones()";
        Top.GetComponent<EndzoneManager>().isSelectable = false;
        Top.GetComponent<EndzoneManager>().PortraitGlowImage.enabled = false;
        Bottom.GetComponent<EndzoneManager>().isSelectable = false;
        Bottom.GetComponent<EndzoneManager>().PortraitGlowImage.enabled = false;

        int dis = (GameManager.Instance.CurrentPlayerTurn.ActionPoints - 1) > 0 ? (GameManager.Instance.CurrentPlayerTurn.ActionPoints - 1) : 0;

        List<OneCardManager> HighlightableCards = GetValidMovesDist(GameManager.Instance.CurrentPlayerTurn.point,
                                               GameManager.Instance.CurrentPlayerTurn.arrows,
                                               dis);

        foreach (OneCardManager cards in HighlightableCards)
        {

            if (IsEndzoneValid(cards.point))
            {
                if (cards.point.Y == 1)
                {
                    BoardManager.Instance.Bottom.GetComponent<EndzoneManager>().PortraitGlowImage.enabled = true;
                }
                else
                {
                    BoardManager.Instance.Top.GetComponent<EndzoneManager>().PortraitGlowImage.enabled = true;
                }
            }
        }



        if (GameManager.Instance.CurrentPlayerTurn.myCardManager != null && GameManager.Instance.CurrentPlayerTurn.ActionPoints > 0)
        {
            if (IsEndzoneValid(GameManager.Instance.CurrentPlayerTurn.myCardManager.point))
            {
                if (GameManager.Instance.CurrentPlayerTurn.myCardManager.point.Y == 0)
                {

                    Top.GetComponent<EndzoneManager>().PortraitGlowImage.enabled = true;
                    Top.GetComponent<EndzoneManager>().isSelectable = true;
                }
                else
                {
                    Debug.Log("bottom");
                    Bottom.GetComponent<EndzoneManager>().PortraitGlowImage.enabled = true;
                    Bottom.GetComponent<EndzoneManager>().isSelectable = true;
                }
            }
        }
    }
    public IEnumerator UpdateBoard()
    {
        yield return new WaitForSeconds(1f); // very strage behavor if I delete this
        yield return StartCoroutine(DealOutFieldCards(0.2f));
        ChatBot2.text = ChatBot2.text + "\n" + System.DateTime.Now.ToString("hh:mm:ss") + ": : UpdateBoard()";
        UpdateCards();

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
