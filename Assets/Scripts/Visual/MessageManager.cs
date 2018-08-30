using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour 
{
    public Text MessageText;
    public GameObject MessagePanel;

    public Image panelImage;
    public Sprite Merlin;
    public Sprite Patric;
    public Sprite youWinPartic;
    public Sprite youWinMerlin;

    public static MessageManager Instance;

    void Awake()
    {
        Instance = this;
        MessagePanel.SetActive(false);
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
                break;
            case "patricWin":
                panelImage.sprite = youWinPartic;
                break;
            case "merlinWin":
                panelImage.sprite = youWinMerlin;
                break;
            case "merlin":
                panelImage.sprite = Merlin;
                break;
        }

     
        //Debug.Log("Showing some message. Duration: " + Duration);
        MessageText.text = Message;
        MessagePanel.SetActive(true);

        yield return new WaitForSeconds(Duration);

        MessagePanel.SetActive(false);
        //Command.CommandExecutionComplete();
    }
    
    // testing only
   
}
