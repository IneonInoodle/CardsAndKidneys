using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HPVisual : MonoBehaviour
{

    public Text HpText;
    public int TestHpThisTurn;

    private int totalHp = 15;
    public int TotalHp
    {
        get { return totalHp; }

        set
        {
            //Debug.Log("Changed total mana to: " + value);

            if (value > HealthPoints.Length)
                totalHp = HealthPoints.Length;
            else if (value < 0)
                totalHp = 0;
            else
                totalHp = value;

            for (int i = 0; i < HealthPoints.Length; i++)
            {
                if (i < totalHp)
                {
                    if (HealthPoints[i].color == Color.clear)
                        HealthPoints[i].color = Color.gray;
                }
                else
                    HealthPoints[i].color = Color.clear;
            }

            // update the text
            //ProgressText.text = string.Format("{0}/{1}", availableCrystals.ToString(), totalAp.ToString());
        }
    }

    private int availableHp;
    public int AvailableHp
    {
        get { return availableHp; }

        set
        {
            //Debug.Log("Changed mana this turn to: " + value);

            if (value > totalHp)
                availableHp = totalHp;
            else if (value < 0)
                availableHp = 0;
            else
                availableHp = value;

            for (int i = 0; i < totalHp; i++)
            {
                if (i < availableHp)
                    HealthPoints[i].color = Color.white;
                else
                    HealthPoints[i].color = Color.gray;
            }

            // update the text
            HpText.text = string.Format("{0}", availableHp.ToString());

        }
    }
    public Image[] HealthPoints;
    //public Text ProgressText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame

  
}
