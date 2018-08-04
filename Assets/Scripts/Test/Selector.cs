using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{

    public bool isSelectable;

    private bool isSelected;
    public bool IsSelected
    {
        get { return isSelected; }
        set { isSelected = value;

            Debug.Log(value);
            card.CardFaceGlowObject.SetActive(isSelected);           
        }
    }

    private SelectionManager sm;
    private OneCardManager card;
    // Use this for initialization
    void Awake()
    {
        isSelectable = false;
        isSelected = false;
        card = gameObject.GetComponent<OneCardManager>();
        sm = UnityEngine.Object.FindObjectOfType<SelectionManager>().GetComponent<SelectionManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        Debug.Log("press");
        if (isSelectable == true)
        {
            Debug.Log("ok ok");
            if (IsSelected == false) // trying to set it to true
            {
                Debug.Log("ok ok ok ");
                if (sm.amountOfCardsRequired > sm.AmountOfCardsSelected)
                {
                    IsSelected = true;
                    sm.AmountOfCardsSelected++;
                }
            } else
            {
                IsSelected = false;
                sm.AmountOfCardsSelected--;  
            }
        }
    }
}