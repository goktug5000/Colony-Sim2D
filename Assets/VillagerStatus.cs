using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VillagerStatus : MonoBehaviour
{
    public string name;
    public float HP,maxHP;
    public float Food;
    public float Rest;
    public float MoodNow;
    public float MoodExpectation;

    public string workingIn;

    public bool amIChosen;
    public static bool AnyChosen;
    public LayerMask layerMask;

    public GameObject chosenSym;

    void Start()
    {
        chosenSym.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ChooseMe();
        ChooseJob();
    }
    void ChooseJob()
    {
        if (amIChosen && Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1))
        { 
            
        }
    }
    void ChooseMe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("mouse clicked");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    amIChosen = true;
                    AnyChosen = true;
                    try
                    {
                        chosenSym.SetActive(true);
                    }
                    catch
                    {

                    }
                    try
                    {
                        VillagerUI.ShowChoosenOnUI();

                    }
                    catch
                    {
                        Debug.LogWarning("VillagerUI.cs missing");
                    }
                }
                else
                {
                    AnyChosen = false;
                    chosenSym.SetActive(false);
                }
                //Debug.Log("Clicked on: " + hit.collider.gameObject.name);
            }
        }




    }
}
