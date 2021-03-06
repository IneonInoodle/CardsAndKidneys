﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Events;
/// <summary>
/// This class will show damage dealt to creatures or payers
/// </summary>

public class DamageEffect : MonoBehaviour {

    // an array of sprites with different blood splash graphics
    public Sprite[] Splashes;
    public ParticleSystem IceCube;
    // a UI image to show the blood splashes
    public Image DamageImage;

    // CanvasGropup should be attached to the Canvas of this damage effect
    // It is used to fade away the alpha value of this effect
    public CanvasGroup cg;

    // The text component to show the amount of damage taken by target like: "-2"
    public Text AmountText;

    
    void Awake()
    {
        
    
        // pick a random image
        DamageImage.sprite = Splashes[UnityEngine.Random.Range(0, Splashes.Length)];
        IceCube.Stop();

    }

    // A Coroutine to control the fading of this damage effect
    private IEnumerator ShowDamageEffect(int amount)
    {
   
        // make this effect non-transparent
        cg.alpha = 1f;
        // wait for 1 second before fading
        yield return new WaitForSeconds(0.05f);

        
      
        if (amount > 0)
        {
            IceCube.Emit(amount - 3);  // Output: 1
            //SoundManager.PlaySound("iceCube");
            yield return new WaitForSeconds(0.5f);
            // gradually fade the effect by changing its alpha value
            //SoundManager.PlaySound("iceCube");
            yield return new WaitForSeconds(0.7f);
            //SoundManager.PlaySound("iceCube");
        }
        else if (amount < 0)
        {
            //SoundManager.PlaySound("hpUp");
            yield return new WaitForSeconds(1.0f);
        }

        while (cg.alpha > 0)
        {   
            cg.alpha -= 0.05f;
            yield return new WaitForSeconds(0.05f);

            
            
            
            //IceCube.emit(1);
        }

        
        // after the effect is shown it gets destroyed.
        Destroy(this.gameObject);
    }
    public static void CreateMoveEffect(GameObject moveeff, Vector3 pos, Vector3 tohere)
    {
        /*todo: 
        create, DONE
        move,  
        destroy,
        */
        GameObject APG = Instantiate(moveeff, pos, Quaternion.identity);
        APG.transform.DOMove(tohere, 1f);
        Destroy(APG, 1.5f);
    }

    
    public void CreatePoisonEffect(GameObject moveeff, Vector3 pos, Vector3 tohere)
    {
        GameObject APG = Instantiate(moveeff, pos, Quaternion.identity);        
        APG.transform.DOMove(tohere, 1f);        
        Destroy(APG, 1.5f); 

    }
    public void CreatePotionEffect(GameObject moveeff, Vector3 pos, Vector3 tohere)
    {
        GameObject APG = Instantiate(moveeff, pos, Quaternion.identity);
        APG.transform.DOMove(tohere, 1f);
        Destroy(APG, 1.5f);

    }
    public static void CreateDamageEffect(GameObject go, int amount)
    {
       
        GameManager gm=GameManager.Instance;
        
        GameObject newDamageEffect = Instantiate(gm.DamagePrefab);
        DamageEffect de = newDamageEffect.GetComponent<DamageEffect>();

        newDamageEffect.transform.SetParent(go.transform, false);
        newDamageEffect.transform.position = go.transform.position;

        
        newDamageEffect.transform.DOLocalRotateQuaternion(newDamageEffect.transform.localRotation * Quaternion.Euler(0, 0, GameManager.Instance.camera.transform.localRotation.eulerAngles.y), 0f);
        newDamageEffect.transform.localScale = new Vector3(-0.43f,0.5f,1);
 
        // Instantiate a DamageEffect from prefab



        // Get DamageEffect component in this new game object
        
        

        if (amount < 0) {
            de.AmountText.color = Color.green;
            de.AmountText.text = "+" + Math.Abs(amount).ToString();
        }
        else if (amount > 0)
        {   
            de.AmountText.color = Color.blue;
            de.AmountText.text = "-" + (amount).ToString();
        }

        // start a coroutine to fade away and delete this effect after a certain time
        de.StartCoroutine(de.ShowDamageEffect(amount));
    }
}
