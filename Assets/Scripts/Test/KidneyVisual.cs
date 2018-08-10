using UnityEngine;
using System.Collections;
using UnityEngine.UI;


[ExecuteInEditMode]
public class KidneyVisual : MonoBehaviour {
    public Text TurnsTilYouLooseText;

    private int turnsTilyouLoose = 2;
    public int TurnsTilyouLoose
    {
        get { return turnsTilyouLoose; }

        set
        {
            turnsTilyouLoose = value;
            TurnsTilYouLooseText.text = string.Format("{0}/{1}", turnsTilyouLoose, 2);
        }
    }
    
    public int testInt;
    private int availableKidneys;
    public int AvailableKidneys
    {
        get { return availableKidneys; }

        set
        {
            //Debug.Log("Changed mana this turn to: " + value);

            if (value < 0)
                availableKidneys = 0;
            else
                availableKidneys = value;


            for (int i = 0; i < 2; i++)
            {
                Debug.Log("KidneysCapture");
                if (i < availableKidneys)
                {
                    if (KidneyAmount[i].color == Color.clear)
                        KidneyAmount[i].color = Color.white;
                }
                else
                    KidneyAmount[i].color = Color.clear;
            }
            // update the text
            
        }
    }
    public Image[] KidneyAmount;
    //public Text ProgressText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame

  
}


