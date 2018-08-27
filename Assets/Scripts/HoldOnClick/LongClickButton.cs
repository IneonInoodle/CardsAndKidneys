using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pointerDown;
    private float pointerDownTimer;
    public TextMeshProUGUI quickTip;
    public GameObject Panel; 
    [SerializeField]
    private float requiredHoldTime;
    
    public UnityEvent onLongClick;
    [SerializeField]
    private Image fillImage;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
        Debug.Log("OnPointerDown");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();
        StartCoroutine(TimeReset());
        Debug.Log("OnPointerUp");
    }

    private void Start()
    {
        /*quickTip = gameObject.AddComponent<TextMeshProUGUI>();
        quickTip.rectTransform.position = new Vector3(-0.08f, 7.33f, -5.6f);
        quickTip.transform.position = new Vector3(-0.08f,7.33f,-5.6f);
        quickTip.transform.localScale = new Vector3(0.03f,0.03f,0.03f);
        quickTip.fontSize = 14f;
        quickTip.color = Color.yellow;
        quickTip.alignment = TextAlignmentOptions.Center;
        quickTip.text = "WWWWWWWWWWWWORK";
        quickTip.alignment = TextAlignmentOptions.Midline;*/
    }
    private void Update()
    {
        
        if (pointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            if (pointerDownTimer >= requiredHoldTime)
            {
                if (onLongClick != null)
                {
                    onLongClick.Invoke();
                    if(quickTip != null)
                    {
                        OneCardManager _cardasset = GetComponent<OneCardManager>();
                        /* _cardasset.cardAsset.name = 
                         * PlayerFat
                         * PlayerThin
                         * Hp
                         * M5
                         * Neutral                         
                          */
                        Debug.Log(_cardasset.cardAsset.name);
                          switch(_cardasset.cardAsset.name)
                        {
                            //-------------------FieldCard3D---------------------
                            case "PlayerFat":
                                quickTip.text = "It's me whos gonna win this game!";                                
                                break;
                            case "PlayerThin":
                                quickTip.text = "I own this game for sure!";
                                break;
                            case "Hp":
                                quickTip.text = "Take 5 health points";
                                break;
                            case "M5":
                                //quickTip.text = "Take Damage equal to amount on card, increases ActionPoints by one";
                                quickTip.text = "Damage with amout on card \nAction Points +1";
                                break;
                            case "Neutral":
                                quickTip.text = "You can move here, don't worry";
                                break;
                            //-------------------SpellCard3D---------------------
                            case "Boost":
                                quickTip.text = "Increases your action points by one";
                                break;
                            case "Poison":
                                quickTip.text = "3 Rounds with 3 Damage each for oposite player";
                                break;
                            case "Potion":
                                quickTip.text = "HP +5";
                                break;
                            case "Replace2":
                                quickTip.text = "destroy 2 cards and replace them";
                                break;
                            case "Rotate":
                                quickTip.text = "rotate the arrows any card";
                                break;
                            case "Swap":
                                quickTip.text = "swap 2 cards spots";
                                break;
                        }                        
                        Panel.SetActive(true);
                        //GameObject.Find("QuickTipPanel").SetActive(true);
                    }
                  
                }
                    

                Reset();
            }
            if (fillImage !=null)
            fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
        }
    }

    private void Reset()
    {
        //quickTip.text = "";
        pointerDown = false;
        pointerDownTimer = 0;
        if (fillImage != null)
            fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    }
    private void EndReset()
    {
        quickTip.text = "";
        pointerDown = false;
        pointerDownTimer = 0;
        if (fillImage != null)
            fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    }

    IEnumerator TimeReset()
    {
        yield return new WaitForSeconds(1);
        if (quickTip != null)
        {
            quickTip.text = "";
            Panel.SetActive(false);
            //GameObject.Find("QuickTipPanel").SetActive(false);
        }
            
        pointerDown = false;
        pointerDownTimer = 0;
        if (fillImage != null)
            fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    }

}