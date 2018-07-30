using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CardSlotManager : MonoBehaviour
{
    public int indY; // row had to do this because can access point struct from unity editor from some reason
    public int indX; // col 

    public Point point;
    public List<GameObject> Kidneys; // when players die, if they are carrying a kidney its dropped to the slot

    private void Awake()
    {
        point.X = indX;
        point.Y = indY;
    }
}
    
