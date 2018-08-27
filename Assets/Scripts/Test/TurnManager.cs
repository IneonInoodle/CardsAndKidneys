using System.Collections;
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
        isTurnComplete = true;

        Debug.Log("hmmmmmm");
        StartCoroutine(GameManager.Instance.UpdateTurn());
        GameManager.Instance.RotateEverything();

    }
	// Update is called once per frame
	void Update () {
		
	}
}
