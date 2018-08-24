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
        //EndReset();
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
                        quickTip.text = "You seems like Lost oder?";
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