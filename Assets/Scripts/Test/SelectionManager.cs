using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SelectionManager : MonoBehaviour {

    //only turn on confirm button when goal of cards selected met
    //only allow certain number of cards to be selected

    GameMangerKelton gm;
    public GameObject blackScreen;
    public GameObject ribbon;
    public Text RibbonText;
    public Button confirmButton;
    public Button cancleButton;
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
    private void Awake()
    {   
        //start of game, this whole thing could be a singleton 
        gm = UnityEngine.Object.FindObjectOfType<GameMangerKelton>().GetComponent<GameMangerKelton>();
        board = BoardManager.Instance;
 
        blackScreen.SetActive(false);
        ribbon.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        cancleButton.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public void FuckMe()
    {
        StartCoroutine(getSelection(2));
    }

    public IEnumerator getSelection(int amount)
    {
        String y = "Choose One Card";

        if (amount == 1)
        {
            y = "Choose One Card";
        } else if (amount == 2)
        {
            y = "Choose Two Cards";
        }

        RibbonText.text = y;
        selectionComplete = false;
        gm.DisableInputs();
        blackScreen.SetActive(true);
        ribbon.SetActive(true);
        confirmButton.gameObject.SetActive(true);
        cancleButton.gameObject.SetActive(true);

        // setting things at start here 
        confirmButton.gameObject.SetActive(false);
        points.Clear();
        amountOfCardsRequired = amount;
        AmountOfCardsSelected = 0;
        AllSelectors = GameObject.FindObjectsOfType<Selector>();

        //turn all field cards to be selectable
        foreach (Selector s in AllSelectors)
        {   
            if (s.gameObject.GetComponent<OneCardManager>().cardAsset.Type != CardType.Player)
            {
                // here theres a check missing for their card type
                s.isSelectable = true;
            }
        }

        fieldCardParent.transform.DOMove(new Vector3(fieldCardParent.transform.position.x, fieldCardParent.transform.position.y + moveDistance, fieldCardParent.transform.position.z), 1.0f);
        yield return new WaitForSeconds(1f);
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
        selectionComplete = true;

        foreach (Selector s in AllSelectors)
        {
            s.IsSelected = false;
            s.isSelectable = false;
            if (s.gameObject.GetComponent<OneCardManager>().cardAsset.Type != CardType.Player)
            {
                // extra check to disable all glowing field cards
                s.gameObject.GetComponent<OneCardManager>().CardFaceGlowObject.SetActive(false);
            }
        }

        ribbon.SetActive(false);
        blackScreen.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        cancleButton.gameObject.SetActive(false);

        gm.EnableInputs();

    }

    public void confirm()
    {
        Debug.Log("confirm");
        StartCoroutine(exit());
    }

    public void cancel()
    {
        Debug.Log("cancle");
        points.Clear();
        StartCoroutine(exit());
    }
}
