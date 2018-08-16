using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragSpellNoTarget: DraggingActions{

    private int savedHandSlot;
    //private WhereIsTheCardOrCreature whereIsCard;

    void Awake()
    {
        //whereIsCard = GetComponent<WhereIsTheCardOrCreature>();
    }

    public override void OnStartDrag()
    {
        savedHandSlot = playerOwner.handvisual.GetIndexOfCard(gameObject);

        playerOwner.handvisual.BringToFront(gameObject);

    }

    public override void OnDraggingInUpdate()
    {
        
    }

    public override void OnEndDrag()
    {
        // 1) Check if we are holding a card over the table
        if (DragSuccessful())
        {
            // play this card
            playerOwner.handvisual.PlayASpellFromHand(gameObject);
        }
        else
        {
            
            //unsucessful
            // Set old sorting order 
            //whereIsCard.Slot = savedHandSlot;

            // Move this card back to its slot position

            //old code
            //Vector3 oldCardPos = playerOwner.handvisual.slots.Children[savedHandSlot].transform.localPosition;
            //transform.DOLocalMove(oldCardPos, 1f);
      
            playerOwner.handvisual.PlaceCardsOnNewSlots();
        } 
    }

    protected override bool DragSuccessful()
    {
        //bool TableNotFull = (TurnManager.Instance.whoseTurn.table.CreaturesOnTable.Count < 8);
        
        return GameManager.Instance.TableCollider.CursorOverThisTable; //&& TableNotFull;
    }


}
