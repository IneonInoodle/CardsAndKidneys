using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class PlayerInput : MonoBehaviour {
    private float h;
    public float H { get { return h; } set { h = value; } }

    private float v;
    public float V { get { return v; } set { v = value; } }

    private bool inputEnabled = false;
    public bool InputEnabled { get { return inputEnabled; } set { inputEnabled = value; } }

    private bool swiping = false;
    private bool eventSent = false;
    private Vector2 lastPosition;

    public bool toucht = true;

    public void GetKeyInput()
    {
        v = 0;
        h = 0;
        if (inputEnabled)
        {   
            if (toucht == true) {
                if (Input.touchCount == 0)
                    return;

                if (Input.GetTouch(0).deltaPosition.sqrMagnitude != 0)
                {
                    if (swiping == false)
                    {
                        swiping = true;
                        lastPosition = Input.GetTouch(0).position;
                        return;
                    }
                    else
                    {
                        if (!eventSent)
                        {
                            Vector2 direction = Input.GetTouch(0).position - lastPosition;

                            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                            {
                                if (direction.x > 0)
                                    h = 1;
                                else
                                    h = -1;
                            }
                            else
                            {
                                if (direction.y > 0)
                                    v = 1;
                                else
                                    v = -1;
                            }

                            eventSent = true;
                        }
                    }
                }
                else
                {
                    swiping = false;
                    eventSent = false;
                }

            } else if(toucht == false)
            {      
                if (inputEnabled)
                {
                    h = Input.GetAxisRaw("Horizontal");
                    v = Input.GetAxisRaw("Vertical");
                }
            }
        }
    }
	// Use this for initialization
	void Start () {
	}
    
	// Update is called once per frame
	void Update () {
	}
}
