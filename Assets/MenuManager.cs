using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{

    public Image BGBlur;
    public Image Kidneys;
    public Image Cards;

    public Image ImageBlack;

    public Image ElectricBoogalooPresents;
    public Image CardsText;
    public Image KidneysText;
    public Image AndText;

    public Image presstoContinueText;
    public Image copyrightText;

    public Camera myCamera;
    public GameObject ZoomLocation;
    public GameObject MenuSceneButtons;

    public void MoveItemsOffScreen(float delay)
    {
        ElectricBoogalooPresents.transform.DOMoveY(300, delay);
        Cards.transform.DOMoveY(500, delay);
        AndText.transform.DOMoveY(500, delay);
        CardsText.transform.DOMoveY(500, delay);

        Kidneys.transform.DOMoveY(-500, delay);
        presstoContinueText.transform.DOMoveY(-500, delay);
        KidneysText.transform.DOMoveY(-500, delay);
        copyrightText.transform.DOMoveY(-500, delay);

    }

    public void zoomCamera(float delay)
    {
        myCamera.transform.DOMove(new Vector3(ZoomLocation.transform.position.x, ZoomLocation.transform.position.y, ZoomLocation.transform.position.z), delay);



    }
    // Use this for initialization
    void Start()
    {
        MenuSceneButtons.SetActive(false);
    }


    public IEnumerator FadeImage(Image i, float f)
    {
        Color myColor = i.color;

        myColor.a = 1f;
        // gradually fade the effect by changing its alpha value
        while (myColor.a > 0)
        {
            myColor.a -= f;
            i.color = myColor;
            yield return new WaitForSeconds(0.01f); // due to float math choose non round 
        }
    }

    public IEnumerator DoStartEffects()
    {
        MoveItemsOffScreen(1);
        StartCoroutine(FadeImage(BGBlur, 0.02f));
        yield return new WaitForSeconds(0.5f);
        zoomCamera(1);
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(FadeImage(ImageBlack, 0.05f));

        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(DoStartEffects());
        }
    }

    private void OnMouseDown()
    {
        MenuSceneButtons.SetActive(true);
        StartCoroutine(DoStartEffects());
    }
}