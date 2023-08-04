using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmPile : MonoBehaviour
{


    public GameObject mySeed;
    public bool someoneOnIt;
    public bool readyToHarvest;
    public bool planted;

    public float maxTime;
    public float myTime;

    public GameObject[] seeds;

    public void Plant()
    {
        Plant(0);
    }
    public void Plant(int seedToPlant)
    {
        planted = true;
        mySeed = seeds[seedToPlant];
        myTime = maxTime;
    }


    public void Harvest()
    {
        Instantiate(mySeed, this.transform.position, Quaternion.identity);

        planted = false;
        mySeed = null;
        myTime = 0;

    }

    void Update()
    {
        if (myTime <= 0 && planted)
        {
            readyToHarvest = true;
        }
        else
        {
            readyToHarvest = false;
        }
        if (planted)
        {
            myTime -= Time.deltaTime;
        }

    }
}
