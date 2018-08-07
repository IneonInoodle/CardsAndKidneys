using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCollider : MonoBehaviour {

    // are we hovering over this table`s collider with a mouse
    private bool cursorOverThisTable = false;

    // A 3D collider attached to this game object
    private BoxCollider col;

    // PROPERTIES

    // returns true if we are hovering over any player`s table collider


    void Awake()
    {
        col = GetComponent<BoxCollider>();
    }


    public bool CursorOverThisTable
    {
        get { return cursorOverThisTable; }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // we need to Raycast because OnMouseEnter, etc reacts to colliders on cards and cards "cover" the table
        // create an array of RaycastHits
        RaycastHit[] hits;
        // raycst to mousePosition and store all the hits in the array
        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30f);

        bool passedThroughTableCollider = false;
        foreach (RaycastHit h in hits)
        {
            // check if the collider that we hit is the collider on this GameObject
            if (h.collider == col)
                passedThroughTableCollider = true;
        }
        cursorOverThisTable = passedThroughTableCollider;
    }
}
