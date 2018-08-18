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
    public Text CardTitleText;
    [Header("Canvas Group")]
    public CanvasGroup cg;
    [Header("Image References")]
    public Image CardTypeImage;
    public Image CardGraphicImage;

    public Image CardBodyImage;
    public Image DamageImage;

    public Image CardArrowUpImage;
    public Image CardArrowDownImage;
    public Image CardArrowLeftImage;
    public Image CardArrowRightImage;

    public Image CardFaceFrameImage;

    public GameObject CardFaceGlowObject;
    public Image CardFaceInnerGlowImage;

    public Image CardGreyOutImage;
    Color normalColor = new Color(196f, 0f, 0f);

    public Point point;

    public arrows arrows = arrows.None;

    public void setRandomArrows()
    {
        int range = 4;

        int rand = Random.Range(0, range);

        switch (rand)
        {
            case 0:
                arrows = arrows.Up | arrows.Left ; 
                break;
            case 1:
                arrows = arrows.Up | arrows.Right;
                break;
            case 2:
                arrows = arrows.Down | arrows.Left ;
                break;
            case 3:
                arrows = arrows.Down | arrows.Right ;
                break;
        }
    }

    public void updateArrows(arrows arrowz) {

        CardArrowLeftImage.enabled = (arrowz & arrows.Left) != 0;
        CardArrowRightImage.enabled = (arrowz & arrows.Right) != 0;
        CardArrowDownImage.enabled = (arrowz & arrows.Down) != 0;
        CardArrowUpImage.enabled = (arrowz & arrows.Up) != 0;
        this.arrows = arrowz;


    }

    public void updateArrowsGlow(arrows arrowz)
    {
        if ((arrowz & arrows.Left) != 0)
        {
            CardArrowLeftImage.color = normalColor;
        } else
        {
            CardArrowLeftImage.color = Color.black;
        }
        if ((arrowz & arrows.Right) != 0)
        {
            CardArrowRightImage.color = normalColor;
        }
        else
        {
            CardArrowRightImage.color = Color.black;
        }
        if ((arrowz & arrows.Up) != 0)
        {
            CardArrowUpImage.color = normalColor;
        }
        else
        {
            CardArrowUpImage.color = Color.black;
        }
        if ((arrowz & arrows.Down) != 0)
        {
            CardArrowDownImage.color = normalColor;
        }
        else
        {
            CardArrowDownImage.color = Color.black;
        }
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

        if (cardAsset.Type != CardType.Spell)
        {
            
            setRandomArrows();
            updateArrows(arrows);//meh
            
        } else
        {
            CardGreyOutImage.enabled = false;
        }
       
        CardFaceInnerGlowImage.enabled = false;
        CardFaceGlowObject.SetActive(false);

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
            CardFaceGlowObject.SetActive(value);

        }
    }

    public void ReadCardFromAsset()
    {

        // 5) Change the card graphic sprite

        CardGraphicImage.sprite = cardAsset.CardImage;
        CardFaceFrameImage.sprite = cardAsset.FrameImage;
        CardBodyImage.sprite = cardAsset.CardBodyImage;



        if (cardAsset.Type == CardType.Monster)
        {
            CardArrowUpImage.sprite = cardAsset.CardArrowImage;
            CardArrowDownImage.sprite = cardAsset.CardArrowImage;
            CardArrowLeftImage.sprite = cardAsset.CardArrowImage;
            CardArrowRightImage.sprite = cardAsset.CardArrowImage;
            DamageImage.sprite = cardAsset.DamageImage;
            DamageText.text = cardAsset.Damage.ToString();
        } else if (cardAsset.Type == CardType.Hp || cardAsset.Type == CardType.Neutral) 
        {
            DamageText.text = cardAsset.Damage.ToString();
            DamageImage.enabled = false;
            DamageText.enabled = false;

            CardArrowUpImage.sprite = cardAsset.CardArrowImage;
            CardArrowDownImage.sprite = cardAsset.CardArrowImage;
            CardArrowLeftImage.sprite = cardAsset.CardArrowImage;
            CardArrowRightImage.sprite = cardAsset.CardArrowImage;

        } else if (cardAsset.Type == CardType.Spell)
        {
            CardTitleText.text = cardAsset.name;
        } else if (cardAsset.Type == CardType.Player)
        {
            DamageImage.enabled = false;
            DamageText.enabled = false;
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
        AudioManager.instance.Play("cardFlip");
        this.GetComponent<DragRotator>().enabled = false;
        this.transform.DOLocalRotate(new Vector3(0f, 360f, this.transform.localRotation.eulerAngles.z), 0.5f, RotateMode.FastBeyond360); // why doesnt it work
       
        yield return null;
    }

    public IEnumerator DeleteThisCard()
    {
        BoardManager.Instance.AllCards.Remove(this);

        // make this effect non-transparent
        cg.alpha = 1f;

        
        // gradually fade the effect by changing its alpha value
        while (cg.alpha > 0)
        {
            cg.alpha -= 0.05f;
            yield return new WaitForSeconds(0.05f); // due to float math choose non round 
        }
        // after the effect is shown it gets destroyed.
        Destroy(this.gameObject);   
    }
}
