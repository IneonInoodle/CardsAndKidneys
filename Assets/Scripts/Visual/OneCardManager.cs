using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

// holds the refs to all the Text, Images on the card
public class OneCardManager : MonoBehaviour {

    public CardAsset cardAsset;
    public OneCardManager PreviewManager;
    [Header("Text Component References")]
    public Text DamageText;
    public Text HpText;
    public Text ActionPointsText;

    public Text CardTitleText;
    [Header("Canvas Group")]
    public CanvasGroup cg;
    [Header("Image References")]
    public Image CardTypeImage;
    public Image CardGraphicImage;
    public Image CardBodyImage;

    public Image CardArrowUpImage;
    public Image CardArrowDownImage;
    public Image CardArrowLeftImage;
    public Image CardArrowRightImage;

    public Image CardFaceFrameImage;
    public Image CardFaceGlowImage;
    public Image CardFaceInnerGlowImage;

    public Point point;

    public arrows arrows = arrows.None;

    public void setRandomArrows()
    {
        int range = 4;

        int rand = Random.Range(0, range);

        switch (rand)
        {
            case 0:
                arrows = arrows.Up | arrows.Left | arrows.Down | arrows.Right; 
                break;
            case 1:
                arrows = arrows.Up | arrows.Left | arrows.Down | arrows.Right;
                break;
            case 2:
                arrows = arrows.Up | arrows.Left | arrows.Down | arrows.Right;
                break;
            case 3:
                arrows = arrows.Up | arrows.Left | arrows.Down | arrows.Right;
                break;
        }
    }

    public void updateArrows(arrows arrowz) {

        CardArrowLeftImage.enabled = (arrowz & arrows.Left) != 0;
        CardArrowRightImage.enabled = (arrowz & arrows.Right) != 0;
        CardArrowDownImage.enabled = (arrowz & arrows.Down) != 0;
        CardArrowUpImage.enabled = (arrowz & arrows.Up) != 0;
        
    }
// field card
//playerCard
private int adjencyBonus;
    public int AdjencyBonus
    {
        get
        {
            return adjencyBonus;
        }
        set
        {
            if (value > 0 && adjencyBonus != value)
            {
                adjencyBonus = value;
                DamageText.text = (cardAsset.Damage + adjencyBonus).ToString();
                DamageText.color = Color.green;
                StartCoroutine(FlipThisCard());
            } 
        }
    }

    //should rename this as it damage not hp ? well kind of 
    private int hp = 10;
    public int Hp
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            //HpText.text = hp.ToString();
            if (hp > 10)
            {
                //HpText.color = Color.magenta;
            }
        }
    }

    void Awake()
    {   
        if (cardAsset != null)
            ReadCardFromAsset();


        setRandomArrows();
        updateArrows(arrows);//meh
        //CardFaceInnerGlowImage.enabled = false;
        CardFaceGlowImage.enabled = false; 
}

    private bool isMoveOption = false;
    public bool IsMoveOption
    {
        get
        {
            return isMoveOption;
        }

        set
        {
            isMoveOption = value;
            CardFaceGlowImage.enabled = value;
        }
    }

    public void ReadCardFromAsset()
    {
        // universal actions for any Card
        // 1) apply tint
        if (cardAsset.characterAsset != null)
        {
            CardBodyImage.color = cardAsset.characterAsset.ClassCardTint;
            CardFaceFrameImage.color = cardAsset.characterAsset.ClassCardTint;
        }
        else
        {
            //CardBodyImage.color = GlobalSettings.Instance.CardBodyStandardColor;
            //CardFaceFrameImage.color = Color.white;
        }
        // 2) add card name

        // 3) add mana cost

        // 4) add description

        // 5) Change the card graphic sprite

        CardGraphicImage.sprite = cardAsset.CardImage;

        if (cardAsset.Damage != 0) // we have field card
        {
            //add text and monster icon
            DamageText.text = cardAsset.Damage.ToString();
            CardTypeImage.sprite = cardAsset.CardTypeImage;

            // set one arrow up/down 50/50 chance
            if (Random.value < 0.5f) { CardArrowUpImage.enabled = false; }
            else { CardArrowDownImage.enabled = false; }

            // set one arrow left or right 50/50 chance
            if (Random.value < 0.5f) { CardArrowLeftImage.enabled = false; }
            else { CardArrowRightImage.enabled = false; }
        } else // we have a spell card or player card
        {
            // test if player card. could you case...
            if (cardAsset.ActionPoints != 0)
            {
                //ActionPoints = cardAsset.ActionPoints;
                Hp = cardAsset.Hp;
            } else {
                //apply spellcard TitelText
                CardTitleText.text = cardAsset.name;
            }
                       
            
            
        }

        if (PreviewManager != null)
        {
            // this is a card and not a preview
            // Preview GameObject will have OneCardManager as well, but PreviewManager should be null there
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.ReadCardFromAsset();
        }
    }


    
   

    public IEnumerator FlipThisCard()
    {
        this.GetComponent<DragRotator>().enabled = false;
        this.transform.DORotate(new Vector3(0f, 360f, 0f), 0.5f, RotateMode.FastBeyond360); // why doesnt it work
        yield return null;
    }

    public IEnumerator DeleteThisCard()
    {
        
        // make this effect non-transparent
        cg.alpha = 1f;

        
        // gradually fade the effect by changing its alpha value
        while (cg.alpha > 0)
        {
            cg.alpha -= 0.05f;
            yield return new WaitForSeconds(0.05f); // due to float math choose non round 
        }
        // after the effect is shown it gets destroyed.
        Debug.Log("destroy");
        Destroy(this.gameObject);   
    }
}
