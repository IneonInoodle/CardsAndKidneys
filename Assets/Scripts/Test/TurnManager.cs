using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {


    protected GameMangerKelton gameManager;

    protected bool isTurnComplete = false;

    public bool IsTurnComplete {  get { return isTurnComplete; } set { isTurnComplete = value; } }
    // Use this for initialization
	protected virtual void Awake () {
        gameManager = Object.FindObjectOfType<GameMangerKelton>().GetComponent<GameMangerKelton>();
	}
	
    public void FinishTurn()
    {
        Debug.Log("finished turn");
        isTurnComplete = true;

        if (gameManager != null)
        {
            StartCoroutine(gameManager.UpdateTurn());
        }
        
    }
	// Update is called once per frame
	void Update () {
		
	}
}
