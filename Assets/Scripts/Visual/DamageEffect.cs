using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using System;

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

    
    }

    // A Coroutine to control the fading of this damage effect
    private IEnumerator ShowDamageEffect(int amount)
    {
        // make this effect non-transparent
        cg.alpha = 1f;
        // wait for 1 second before fading
        yield return new WaitForSeconds(0.15f);
        IceCube.Emit(amount);
        IceCube.Stop();
        yield return new WaitForSeconds(1f);
        // gradually fade the effect by changing its alpha value

 
        Debug.Log("amount" + amount);

        while (cg.alpha > 0)
        {   
            cg.alpha -= 0.05f;
            yield return new WaitForSeconds(0.05f);

            
            
            
            //IceCube.emit(1);
        }
        Debug.Log("zou");
        
        // after the effect is shown it gets destroyed.
        Destroy(this.gameObject);
    }

   
    public static void CreateDamageEffect(GameObject go, int amount)
    {
       
        GameMangerKelton gm;
        gm = UnityEngine.Object.FindObjectOfType<GameMangerKelton>().GetComponent<GameMangerKelton>(); // could
        
        GameObject newDamageEffect = Instantiate(gm.DamagePrefab);
        DamageEffect de = newDamageEffect.GetComponent<DamageEffect>();

        newDamageEffect.transform.SetParent(go.transform, false);
        newDamageEffect.transform.position = go.transform.position;
        Debug.Log(amount);
 
        // Instantiate a DamageEffect from prefab



        // Get DamageEffect component in this new game object
        
        Debug.Log("what");

        if (amount < 0) {
            de.AmountText.color = Color.green;
            de.AmountText.text = "+" + Math.Abs(amount).ToString();
        }
        else if (amount > 0)
        {
            Debug.Log("what" + amount);
            de.AmountText.color = Color.blue;
            de.AmountText.text = "-" + (amount).ToString();
        }

        // start a coroutine to fade away and delete this effect after a certain time
        de.StartCoroutine(de.ShowDamageEffect(Math.Abs(amount)));
    }
}
