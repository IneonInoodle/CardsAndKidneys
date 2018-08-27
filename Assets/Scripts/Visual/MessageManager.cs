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
        if (Message == "patric")
        {
            panelImage.sprite = Patric;
        } else
        {
            panelImage.sprite = Merlin;
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
