using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsEffect_Color_Editor : MonoBehaviour {

    public Material MatObject1;
    public Color color1 = Color.black;

    public Material MatObject2;
    public Color color2 = Color.black;

    public Material MatObject3;
    public Color color3 = Color.black;

    public Material MatObject4;
    public Color color4 = Color.black;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        color1 = Color.red;
	}
}
