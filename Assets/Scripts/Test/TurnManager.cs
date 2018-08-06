﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

    protected bool isTurnComplete = false;

    public bool IsTurnComplete {  get { return isTurnComplete; } set { isTurnComplete = value; } }
    // Use this for initialization
	protected virtual void Awake () {
	}
	
    public void FinishTurn()
    {
        Debug.Log("finished turn");
        isTurnComplete = true;


        StartCoroutine(GameManager.Instance.UpdateTurn());

        
    }
	// Update is called once per frame
	void Update () {
		
	}
}
