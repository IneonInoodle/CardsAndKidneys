using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour {


    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private Vector2 startTouch, swipeDelta;
    private bool isDraging = false;
    public bool Tap { get { return tap; } }
    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight{ get { return swipeRight; } }
    public bool SwipeUp{ get { return swipeUp; } }
    public bool SwipeDown{ get { return swipeDown; } }

    private void Update()
    {
        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

        #region Standalone Inputs
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            isDraging = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDraging = false;
            Reset();
        }
        #endregion

        #region Mobile Inputs
        if(Input.touches.Length != 0)
        {
            if(Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                isDraging = true;
                startTouch = Input.touches[0].position;
            }
            else if(Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                isDraging = false;
                Reset();
            }
        }
        #endregion

        swipeDelta = Vector2.zero;
        if(isDraging)
        {
            if(Input.touches.Length != 0)
            {
                swipeDelta = Input.touches[0].position - startTouch;
            }
            else if (Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
            //did cross the deadzone?
            if (swipeDelta.magnitude > 125)
            {//which direction?
                float x = swipeDelta.x;
                float y = swipeDelta.y;
                if(Mathf.Abs(x) > Mathf.Abs(y))
                {//left or right
                    if (x < 0)
                    {
                        Debug.Log("X_left: " + x);
                        swipeLeft = true;
                    }
                    else
                        Debug.Log("X_right: " + x);
                    swipeRight = true;
                } else
                {//up or down
                    if (y < 0)
                    {
                        Debug.Log("Y_down: " + x);
                        swipeDown = true;
                    }
                    else
                        Debug.Log("Y_up: " + x);
                    swipeUp = true;

                }
                Reset();
            }
        }
    }

    private void Reset()
    {
        
        startTouch = swipeDelta = Vector2.zero;
    }
}
