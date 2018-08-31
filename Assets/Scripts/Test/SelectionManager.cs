using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SelectionManager : MonoBehaviour {

    //only turn on confirm button when goal of cards selected met
    //only allow certain number of cards to be selected
    
    public GameObject blackScreen;
    public GameObject ribbon;
    public Image ribbonImage;

    public Sprite choose1;
    public Sprite choose2;

    public Text RibbonText;
    public Text ChatBot2;
    public Button confirmButton;

    public bool isSelectingForSpellCards = false;
    public GameObject fieldCardParent;

    public List<Point> points = new List<Point>();

    public int moveDistance;

    private BoardManager board;
    private bool selectionComplete = false;


    private int amountOfCardsSelected = 0;
    public int AmountOfCardsSelected
    {
        get { return amountOfCardsSelected; }
        set
        {
            amountOfCardsSelected = value;
            confirmButton.gameObject.SetActive(amountOfCardsSelected == amountOfCardsRequired);
            
        }
    }
    public int amountOfCardsRequired = 2;

    Selector[] AllSelectors;
    private void Start()
    {   
        //start of game, this whole thing could be a singleton 
        board = BoardManager.Instance;
        blackScreen.SetActive(false);
        ribbon.SetActive(false);
        confirmButton.gameObject.SetActive(false);


    }

    // Use this for initialization
    void St_art () {
        
    }
	

   public void HighlightValidMoves(PlayerManager p)
    {
        
    }
	// Update is called once per frame
	void Update () {
		
	}


    public void FuckMe()
    {
        StartCoroutine(getSelectionWithPlayers(2));
    }

    public IEnumerator getSelectionWithPlayers(int amount)
    {
        isSelectingForSpellCards = true;
        String y = "Choose 1 Card";

        if (amount == 1)
        {
            y = "Choose 1 Card";
            ribbonImage.sprite = choose1;
        }
        else if (amount == 2)
        {
            y = "Choose 2 Cards";
            ribbonImage.sprite = choose2;
        }

        RibbonText.text = y;

        selectionComplete = false;
        GameManager.Instance.DisableInputs();
        
   


        // setting things at start here 
        confirmButton.gameObject.SetActive(false);
        points.Clear();
        amountOfCardsRequired = amount;
        AmountOfCardsSelected = 0;
        AllSelectors = GameObject.FindObjectsOfType<Selector>();

        //turn all field cards to be selectable
        foreach (Selector s in AllSelectors)
        {

            var bb = s.gameObject.GetComponentsInChildren<Canvas>();

            foreach (Canvas cc in bb)
            {
                Debug.Log(cc.name);
                switch (cc.name)
                {
                    case "Canvas": // this would be the kidney on le card

                        break;
                    case "CanvasFieldCard":
                        cc.sortingLayerName = "Selection";
                        cc.sortingOrder = 1;
                        break;
                    case "DefaultIdol":
                        cc.sortingLayerName = "Selection";
                        cc.sortingOrder = 10;
                        break;
                    case "CardGlowObject":
                        cc.sortingLayerName = "Selection";
                        cc.sortingOrder = 11;
                        break;
                }
            }

            var bbb = s.gameObject.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer ccc in bbb)
            {
                Debug.Log("ggtttt");
                ccc.sortingLayerName = "Selection";
                ccc.sortingOrder = 10;
            }

           
                // here theres a check missing for their card type
                s.isSelectable = true;

                ParticleSystemRenderer[] psr = s.GetComponentsInChildren<ParticleSystemRenderer>(true);

                foreach (ParticleSystemRenderer p in psr)
                {
                    p.sortingLayerName = "Selection";
                    p.sortingOrder = 2;
                }
            
        }

        blackScreen.SetActive(true);
        ribbon.SetActive(true);

        fieldCardParent.transform.DOMove(new Vector3(fieldCardParent.transform.position.x, fieldCardParent.transform.position.y + moveDistance, fieldCardParent.transform.position.z), 0.5f);
        yield return new WaitForSeconds(0.5f);

        
        while (!selectionComplete) // loop to stay in coroutine until done
        {

            //check for game over condition

            //win
            //reach end of the level
            // lose
            //player dies
            // IsGameOver = true; 

            yield return null;
        }
        //enable clicks on field cards

        // glow on click
        //store clicks in selection manager
        //
    }

    public IEnumerator getSelectionNoPlayers(int amount)
    {
        isSelectingForSpellCards = true;
        String y = "Choose 1 Card";

        if (amount == 1)
        {
            y = "Choose 1 Card";
            ribbonImage.sprite = choose1;
        }
        else if (amount == 2)
        {
            y = "Choose 2 Cards";
            ribbonImage.sprite = choose2;
        }

        RibbonText.text = y;

        selectionComplete = false;
        GameManager.Instance.DisableInputs();




        // setting things at start here 
        confirmButton.gameObject.SetActive(false);
        points.Clear();
        amountOfCardsRequired = amount;
        AmountOfCardsSelected = 0;
        AllSelectors = GameObject.FindObjectsOfType<Selector>();

        //turn all field cards to be selectable
        foreach (Selector s in AllSelectors)
        {
            
            // here theres a check missing for their card type
            if (s.gameObject.GetComponent<OneCardManager>().cardAsset.Type != CardType.Player)
            {
                var bb = s.gameObject.GetComponentsInChildren<Canvas>();

                foreach (Canvas cc in bb)
                {
                    switch (cc.name)
                    {
                        case "Canvas": // this would be the kidney on le card

                            break;
                        case "CanvasFieldCard":
                            cc.sortingLayerName = "Selection";
                            cc.sortingOrder = 1;
                            break;
                        case "DefaultIdol":
                            cc.sortingLayerName = "Selection";
                            cc.sortingOrder = 10;
                            break;
                        case "CardGlowObject":
                            cc.sortingLayerName = "Selection";
                            cc.sortingOrder = 11;
                            break;
                    }
                }

                var bbb = s.gameObject.GetComponentsInChildren<SpriteRenderer>();

                foreach (SpriteRenderer ccc in bbb)
                {
                    Debug.Log("ggtttt");
                    ccc.sortingLayerName = "Selection";
                    ccc.sortingOrder = 10;
                }
                s.isSelectable = true;

                ParticleSystemRenderer[] psr = s.GetComponentsInChildren<ParticleSystemRenderer>(true);

                foreach (ParticleSystemRenderer p in psr)
                {
                    p.sortingLayerName = "Selection";
                    p.sortingOrder = 2;
                }
            }
                

            

        }

        blackScreen.SetActive(true);
        ribbon.SetActive(true);

        fieldCardParent.transform.DOMove(new Vector3(fieldCardParent.transform.position.x, fieldCardParent.transform.position.y + moveDistance, fieldCardParent.transform.position.z), 0.5f);
        yield return new WaitForSeconds(0.5f);


        while (!selectionComplete) // loop to stay in coroutine until done
        {

            //check for game over condition

            //win
            //reach end of the level
            // lose
            //player dies
            // IsGameOver = true; 

            yield return null;
        }
        //enable clicks on field cards

        // glow on click
        //store clicks in selection manager
        //
    }

    public IEnumerator exit()
    {
        
        fieldCardParent.transform.DOMove(new Vector3(0, 0, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);
        

        foreach (Selector s in AllSelectors)
        {
            s.IsSelected = false;
            s.gameObject.GetComponent<OneCardManager>().CardFaceInnerGlowImage.color = Color.white;
            s.isSelectable = false;

                // extra check to disable all glowing field cards
                s.gameObject.GetComponent<OneCardManager>().CardFaceGlowObject.SetActive(false);



                var bb = s.gameObject.GetComponentsInChildren<Canvas>();
                

                foreach (Canvas cc in bb)
                {
                    switch (cc.name)
                    {
                        case "Canvas": // this would be the kidney on le card

                            break;
                        case "CanvasFieldCard":
                            cc.sortingLayerName = "FieldCard";
                            cc.sortingOrder = 8;
                            break;
                        case "DefaultIdol":
                            cc.sortingLayerName = "FieldCard";
                            cc.sortingOrder = 10;
                            break;
                        case "CardGlowObject": //this one doesnt matter
                            cc.sortingLayerName = "FieldCard";
                            cc.sortingOrder = 11;
                            break;
                    }
                }

                var bbb = s.gameObject.GetComponentsInChildren<SpriteRenderer>();

                foreach (SpriteRenderer ccc in bbb) //this is for the card image / animation
                {
                    ccc.sortingLayerName = "FieldCard";
                    ccc.sortingOrder = 10;
                }


            ParticleSystemRenderer[] psr = s.GetComponentsInChildren<ParticleSystemRenderer>(true);

                foreach (ParticleSystemRenderer p in psr)
                {
                    p.sortingLayerName = "FieldCard";
                    p.sortingOrder = 9;
                }
        }

        ribbon.SetActive(false);
        blackScreen.SetActive(false);
        confirmButton.gameObject.SetActive(false);

        selectionComplete = true;
        isSelectingForSpellCards = false;
    }

    public void confirm()
    {
        StartCoroutine(exit());
    }

    public void cancel()
    {        
        StartCoroutine(exit());
    }
}
