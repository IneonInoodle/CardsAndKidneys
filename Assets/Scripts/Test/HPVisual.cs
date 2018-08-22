using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[ExecuteInEditMode]
public class HPVisual : MonoBehaviour
{

    public Text HpText;
    public int TestHpThisTurn;

    public Image[] HealthPoints;
    public Image[] IceEffect;
    public Image Box;
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

    private int effectAmount; //damage is positive
    public int EffectAmount
    {
        get { return effectAmount; }

        set
        {
            effectAmount = value;
            Debug.Log(effectAmount);
            if (effectAmount > 0) //posion
            {
                Debug.Log("im poisoned");
                Box.color = Color.black;
                for (int i = availableHp - 1; i > availableHp - effectAmount; i--)
                {
                    if (i >= 0)
                        HealthPoints[i].color = Color.black;
                }
            }
            else if (effectAmount == 0)
            {
                Box.color = Color.red;
                AvailableHp = AvailableHp;
            }
            else {
                Box.color = Color.green;
                for (int i = availableHp; i < availableHp - effectAmount; i++)
                {
                    if (i < 15)
                        HealthPoints[i].color = Color.green;
                }
            }
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
                {
                    HealthPoints[i].color = Color.red;
                    if (i != 0)
                    IceEffect[(int)(i / 5)].color = Color.white; // 15/6 = 2.5
                }
                   
                else
                {
                    HealthPoints[i].color = Color.clear;
                    IceEffect[(int)(i / 5)].color = Color.clear;
                }
                    
            }
 
            // update the text
            HpText.text = string.Format("{0}", availableHp.ToString());

        }
    }

    //public Text ProgressText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame

  
}
