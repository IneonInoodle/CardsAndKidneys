using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{

    public List<OneCardManager> PathToMe = new List<OneCardManager>();

    public bool isSelectable;

    private bool isSelected;
    public bool IsSelected
    {
        get { return isSelected; }
        set { isSelected = value;
            card.CardFaceInnerGlowImage.color = Color.green;
            card.CardFaceInnerGlowImage.enabled = isSelected;        
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
        
        if (isSelectable == true)
        {
            if (sm.isSelectingForSpellCards == true) // selecting for a spell
            {

                if (IsSelected == false) // trying to set it to true
                {
                    if (sm.amountOfCardsRequired > sm.AmountOfCardsSelected)
                    {
                        sm.points.Add(card.point);
                        IsSelected = true;
                        sm.AmountOfCardsSelected++;
                    }
                }
                else if (IsSelected == true)
                {
                    Debug.Log("remove");
                    IsSelected = false;
                    sm.points.Remove(card.point);
                    sm.AmountOfCardsSelected--;
                }
            }
            else //Selecting a for a move
            {
                Debug.Log("click to move");
                GameManager.Instance.CurrentPlayerTurn.playerMover.MoveToPoint(card.point);
            }
        }
    }
}