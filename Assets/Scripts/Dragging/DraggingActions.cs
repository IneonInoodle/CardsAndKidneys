using UnityEngine;
using System.Collections;

public abstract class DraggingActions : MonoBehaviour {

    public abstract void OnStartDrag();

    public abstract void OnEndDrag();

    public abstract void OnDraggingInUpdate();

    public virtual bool CanDrag
    {
        get
        {
            return true; //GlobalSettings.Instance.CanControlThisPlayer(playerOwner);
        }
    }


    
    
    protected virtual PlayerManager playerOwner
    {
        get{

            GameManager gm =GameManager.Instance;

            if (tag.Contains("Bottom"))
            {
                foreach (PlayerManager pl in gm.players)
                {
                    if (pl.mySide == location.bottom)
                    {
                        return pl;
                    }
                }
            }     
            else if (tag.Contains("Top")) {
                foreach (PlayerManager pl in gm.players)
                {
                    if (pl.mySide == location.top)
                    {
                        return pl;
                    }
                }
            }   
            else
            {
                Debug.LogError("Untagged Card or creature " + transform.parent.name);
                return null;
            }

            return null;
        }
    }
    
    protected abstract bool DragSuccessful();
}
