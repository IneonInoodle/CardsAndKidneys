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

    public GameObject CardArrowUp;
    public GameObject CardArrowDown;
    public GameObject CardArrowLeft;
    public GameObject CardArrowRight;

    public GameObject CardFaceGlowObject;

    public Image CardFaceInnerGlowImage;
    public Image CardGreyOutImage;

    public Material arrowActiveMat;
    public Material arrowInactiveMat;
    public Material cardFrameMat;

    public GameObject frame;

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

        CardArrowLeft.SetActive((arrowz & arrows.Left) != 0);
        CardArrowRight.SetActive((arrowz & arrows.Right) != 0);
        CardArrowDown.SetActive((arrowz & arrows.Down) != 0);
        CardArrowUp.SetActive((arrowz & arrows.Up) != 0);

        this.arrows = arrowz;
    }

    public void updateArrowsGlow(arrows arrowz)
    {
        if ((arrowz & arrows.Left) != 0)
        {
            CardArrowLeft.GetComponent<MeshRenderer>().material = arrowActiveMat;
           
        } else
        {
            CardArrowLeft.GetComponent<MeshRenderer>().material = arrowInactiveMat;
        }
        if ((arrowz & arrows.Right) != 0)
        {
            CardArrowRight.GetComponent<MeshRenderer>().material = arrowActiveMat;
        }
        else
        {
            CardArrowRight.GetComponent<MeshRenderer>().material = arrowInactiveMat;
        }
        if ((arrowz & arrows.Up) != 0)
        {
            CardArrowUp.GetComponent<MeshRenderer>().material = arrowActiveMat;
        }
        else
        {
            CardArrowUp.GetComponent<MeshRenderer>().material = arrowInactiveMat;
        }
        if ((arrowz & arrows.Down) != 0)
        {
            CardArrowDown.GetComponent<MeshRenderer>().material = arrowActiveMat;
        }
        else
        {
            CardArrowDown.GetComponent<MeshRenderer>().material = arrowInactiveMat;          
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
            CardFaceInnerGlowImage.enabled = false;

        } else
        {
            CardGreyOutImage.enabled = false;
        }
       
        
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
        CardGraphicImage.sprite = cardAsset.CardImage;
        CardBodyImage.sprite = cardAsset.CardBodyImage;

        cardFrameMat = cardAsset.FrameMat;
        if (GameManager.Instance.CurrentPlayerTurn.mySide == location.bottom && cardAsset.Type == CardType.Spell)
        {
            cardFrameMat = GameManager.Instance.BottomSpellMat;
        } else if (GameManager.Instance.CurrentPlayerTurn.mySide == location.top && cardAsset.Type == CardType.Spell)
        {
            cardFrameMat = GameManager.Instance.TopSpellMat;
        }

        frame.GetComponent<MeshRenderer>().material = cardFrameMat;



        if (cardAsset.Type == CardType.Monster)
        {
            arrowInactiveMat = cardAsset.ArrowInactiveMat;
            arrowActiveMat = cardAsset.ArrowActiveMat;

            DamageImage.sprite = cardAsset.DamageImage;
            DamageText.text = cardAsset.Damage.ToString();
        } else if (cardAsset.Type == CardType.Hp || cardAsset.Type == CardType.Neutral) 
        {

            arrowInactiveMat = cardAsset.ArrowInactiveMat;
            arrowActiveMat = cardAsset.ArrowActiveMat;

            DamageText.text = cardAsset.Damage.ToString();
            DamageImage.enabled = false;
            DamageText.enabled = false;



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
        this.transform.DOLocalRotate(new Vector3(0f, 360f + 180f, this.transform.localRotation.eulerAngles.z), 0.5f, RotateMode.FastBeyond360); // why doesnt it work
       
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
