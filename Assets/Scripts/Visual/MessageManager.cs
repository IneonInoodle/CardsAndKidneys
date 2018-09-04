using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour 
{
    public Text MessageText;
    public GameObject MessagePanel;

    public Image panelImage;
    public Image YouWin;
    public Sprite Merlin;
    public Sprite Patric;
    public Sprite youWinPartic;
    public Sprite youWinMerlin;

    public static MessageManager Instance;

    void Awake()
    {
        Instance = this;
        MessagePanel.SetActive(false);
        YouWin.enabled = false;
    }

    public void ShowMessage(string Message, float Duration)
    {
        StartCoroutine(ShowMessageCoroutine(Message, Duration));
    }

    IEnumerator ShowMessageCoroutine(string Message, float Duration)
    {   
        switch (Message)
        {
            case "patric":
                panelImage.sprite = Patric;
                panelImage.enabled = true;
                break;
            case "patricWin":
                YouWin.sprite = youWinPartic;
                YouWin.enabled = true;
                break;
            case "merlinWin":
                YouWin.sprite = youWinMerlin;
                YouWin.enabled = true;
                break;
            case "merlin":
                panelImage.sprite = Merlin;
                panelImage.enabled = true;
                break;
        }


        MessagePanel.SetActive(true);
        //Debug.Log("Showing some message. Duration: " + Duration);
        MessageText.text = Message;
        

        yield return new WaitForSeconds(Duration);

        panelImage.enabled = false;
        MessagePanel.SetActive(false);
        YouWin.enabled = false;
        //Command.CommandExecutionComplete();
    }
    
    // testing only
   
}
