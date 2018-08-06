using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// This class enables Drag and Drop Behaviour for the game object it is attached to. 
/// It uses other script - DraggingActions to determine whether we can drag this game object now or not and 
/// whether the drop was successful or not.
/// </summary>

public class Draggable : MonoBehaviour
{

    // PRIVATE FIELDS

    // a flag to know if we are currently dragging this GameObject
    private bool dragging = false;

    // distance from the center of this Game Object to the point where we clicked to start dragging 
    private Vector3 pointerDisplacement;

    // reference to DraggingActions script. Dragging Actions should be attached to the same GameObject.
    private DraggingActions da;

    // STATIC property that returns the instance of Draggable that is currently being dragged
    private static Draggable _draggingThis;
    public static Draggable DraggingThis
    {
        get { return _draggingThis; }
    }

    // MONOBEHAVIOUR METHODS
    void Awake()
    {
        da = GetComponent<DraggingActions>();
    }

    void OnMouseDown()
    {
        if (da != null && da.CanDrag)
        {
            dragging = true;
            // when we are dragging something, all previews should be off
            HoverPreview.PreviewsAllowed = false;
            _draggingThis = this;
            da.OnStartDrag();
            pointerDisplacement =  transform.position - MouseInWorldCoords();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            Vector3 mousePos = MouseInWorldCoords();
            //Debug.Log(mousePos);
            transform.position = new Vector3( mousePos.x + pointerDisplacement.x , transform.position.y,mousePos.z + pointerDisplacement.z);
            da.OnDraggingInUpdate();
        }
    }

    void OnMouseUp()
    {
        if (dragging)
        {
            dragging = false;
            // turn all previews back on
            HoverPreview.PreviewsAllowed = true;
            _draggingThis = null;
            da.OnEndDrag();
        }
    }

    // returns mouse position in World coordinates for our GameObject to follow. 
    private Vector3 MouseInWorldCoords()
    {
        var screenMousePos = Input.mousePosition;
        Vector3 res= Camera.main.ScreenToWorldPoint(screenMousePos);
        Ray ray= Camera.main.ScreenPointToRay(screenMousePos);


        Plane hPlane = new Plane(Vector3.up, Vector3.zero);
        // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
        float distance = 0;
        // if the ray hits the plane...
        if (hPlane.Raycast(ray, out distance))
        {
            // get the hit point:
            res = ray.GetPoint(distance);
        }


        Debug.Log(screenMousePos + "---"+res+"----"+ ray);

        return res;
    }

}
