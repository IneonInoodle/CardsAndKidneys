using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeTest : MonoBehaviour {

    public Swipe swipeControls;
    public Transform player;
    private Vector3 desiredPosition;
    private float startTouch;
    private PagesScript pagesScript;

    // Update is called once per frame
    void Update () {
        GameObject Tuto = GameObject.Find("TutorialMenu");
        if (Tuto != null && Tuto.activeSelf)
        {
            if (swipeControls.SwipeLeft)
                Debug.Log("l!!");
            desiredPosition += Vector3.left;
            if (swipeControls.SwipeRight)
                Debug.Log("r!!");
            desiredPosition += Vector3.right;
            if (swipeControls.SwipeUp)
                Debug.Log("f!!");
            desiredPosition += Vector3.forward;
            if (swipeControls.SwipeDown)
                Debug.Log("b!!");
            desiredPosition += Vector3.back;

            player.transform.position = Vector3.MoveTowards(player.transform.position, desiredPosition, 3f * Time.deltaTime);
            #region Standalone Inputs
            if (Input.GetMouseButtonDown(0))
            {
                startTouch = Input.mousePosition.x;
            }
            else if (Input.GetMouseButtonUp(0))
            {

                startTouch = Input.mousePosition.x;
            }
            #endregion

            #region Mobile Inputs
            if (Input.touches.Length != 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    startTouch = Input.touches[0].position.x;
                }
                else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                {
                    startTouch = Input.touches[0].position.x;
                }
            }
            #endregion
            if (swipeControls.Tap)
            {
                GameObject tuto = GameObject.Find("TutorialMenu");
                if (tuto != null)
                    pagesScript = tuto.GetComponent<PagesScript>();
                if (startTouch > Screen.width / 2)
                {
                    //right
                    pagesScript.NextImage();
                }
                else
                {
                    //left                
                    pagesScript.PreviousImage();
                }


                Debug.Log("tab " + startTouch);
            }
        }
           
	}
}
