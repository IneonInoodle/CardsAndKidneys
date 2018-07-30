using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
    private float h;
    public float H { get { return h; } set { h = value; } }

    private float v;
    public float V { get { return v; } set { v = value; } }

    private bool inputEnabled = false;
    public bool InputEnabled { get { return inputEnabled; } set { inputEnabled = value; } }
    public void GetKeyInput()
    {
        if (inputEnabled)
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }

        
    }
	// Use this for initialization
	void Start () {
	}
    
	// Update is called once per frame
	void Update () {
	}
}
