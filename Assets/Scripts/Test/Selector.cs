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
        Debug.Log("what");
        if (isSelectable == true)
        {

            Debug.Log("what");
            if (IsSelected == false) // trying to set it to true
            {
                Debug.Log(sm.amountOfCardsRequired);
                Debug.Log(sm.AmountOfCardsSelected);
                if (sm.amountOfCardsRequired > sm.AmountOfCardsSelected)
                {
                    Debug.Log("add");
                    sm.points.Add(card.point);
                    IsSelected = true;
                    sm.AmountOfCardsSelected++;
                }
            } else if (IsSelected == true)
            {
                Debug.Log("remove");
                IsSelected = false;
                sm.points.Remove(card.point);
                sm.AmountOfCardsSelected--;
            }
        }
    }
}