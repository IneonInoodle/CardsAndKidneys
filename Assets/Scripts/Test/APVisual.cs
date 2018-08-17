using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class APVisual : MonoBehaviour {

    private int totalAp = 4;
    public int TotalAp
    {
        get { return totalAp; }

        set
        {
            //Debug.Log("Changed total mana to: " + value);

            if (value > ActionPoints.Length)
                totalAp = ActionPoints.Length;
            else if (value < 0)
                totalAp = 0;
            else
                totalAp = value;

            for (int i = 0; i < ActionPoints.Length; i++)
            {
                if (i < totalAp)
                {
                    if (ActionPoints[i].color == Color.clear)
                        ActionPoints[i].color = Color.gray;
                }
                else
                    ActionPoints[i].color = Color.clear;
            }

            // update the text
            //ProgressText.text = string.Format("{0}/{1}", availableCrystals.ToString(), totalAp.ToString());
        }
    }

    private int availableAp;
    public int AvailableAp

    {
        get { return availableAp; }

        set
        {
            //Debug.Log("Changed mana this turn to: " + value);
            if (availableAp > totalAp) //removing extra AP from bar AFTER BOOST
            {
                for (int i = totalAp; i < availableAp; i++)
                {
                    ActionPoints[i].color = Color.clear;
                }
            }
 
            if (value < 0)
                availableAp = 0;
            else
                availableAp = value;


            if (availableAp <= totalAp) //NORMAL SITUATION
            {
                for (int i = 0; i < totalAp; i++)
                {
                    if (i < availableAp)
                        ActionPoints[i].color = Color.white;
                    else
                        ActionPoints[i].color = Color.gray;
                }
            } else //ADDING IN EXTRA AP FROM BOOST
            {
                for (int i = 0; i < availableAp; i++)
                {
                        ActionPoints[i].color = Color.white;                   
                }
            }
            

            // update the text
            //ProgressText.text = string.Format("{0}/{1}", availableCrystals.ToString(), totalAp.ToString());

        }
    }
    public Image[] ActionPoints;
    //public Text ProgressText;

    // Use this for initialization
    void Start () {
		
	}
    // Update is called once per frame

}
