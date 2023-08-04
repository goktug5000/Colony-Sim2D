using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VillagerJobPriority2D : MonoBehaviour
{
    [SerializeField] private VillagerMove2D myVillagerMove;
    [SerializeField] private GameObject chosenSym;
    [SerializeField] private bool amIChosen;
    [SerializeField] private bool onDuty;
    [SerializeField] private bool godLevelOrder;

    public LayerMask layerMask;

    [Header("Mesleki Puanlar")]
    [SerializeField] public allJobs[] myJobs;
    [SerializeField] private string doingNow;
    public enum allJobs
    {
        Cooker,
        Hunter,
        Grower,
        plantCuter,
        lumberJack,
        Hauler,
        Idle,
        Fun
    }
    [SerializeField] private float lumberJackCD;
    [SerializeField] private float lumberJackHit;

    void Start()
    {
        if (myVillagerMove == null)
        {
            myVillagerMove = this.gameObject.GetComponent<VillagerMove2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        regularJob();
    }
    public void regularJob()//tanrý baþka görev vermiyorsa bunlarý yapýyo
    {
        if (onDuty || godLevelOrder)
        {
            return;
        }
        foreach(allJobs theJob in myJobs)
        {
            switch (theJob)
            {
                case allJobs.plantCuter:
                    {
                        if (lookForFarmForHarvest() != null)
                        {
                            return;
                        }
                        break;
                    }
                case allJobs.Grower:
                    {
                        if (lookForFarm() != null)
                        {
                            return;
                        }
                        break;
                    }
                case allJobs.lumberJack:
                    {
                        if (lookForTree() != null)
                        {
                            return;
                        }
                        break;
                    }
                case allJobs.Hauler:
                    {
                        if (lookForHaul() != null)
                        {
                            return;
                        }
                        break;
                    }
            }
        }
        
        
        




    }

    public GameObject lookForStockPile()
    {
        stockPileCode[] stockPileCodes = FindObjectsOfType<stockPileCode>();
        GameObject enYakinObj = null;

        float distanceMin = Mathf.Infinity;
        for (int i = 0; i < stockPileCodes.Length; i++)
        {

            float distance = myVillagerMove.CalculatePathDistance(stockPileCodes[i].gameObject.transform.position);
            if (distance < distanceMin)
            {
                distanceMin = distance;
                enYakinObj = stockPileCodes[i].gameObject;
            }
            //Debug.Log("Distance between " + this.name + " and " + stockPileCodes[i].name + " is " + distance + " units.");

        }

        return enYakinObj;

    }
    public GameObject lookForHaul()
    {

        BlockHolder[] blockHolders = FindObjectsOfType<BlockHolder>();
        GameObject enYakinObj = null;

        float distanceMin = Mathf.Infinity;
        for (int i = 0; i < blockHolders.Length; i++)
        {
            if (blockHolders[i].inPile == false && blockHolders[i].onHaul == false && blockHolders[i].someoneOnIt == false)
            {
                float distance = myVillagerMove.CalculatePathDistance(blockHolders[i].transform.position);
                //Debug.Log("Distance between " + this.name + " and " + blockHolders[i].name + " is " + distance + " units.");
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    enYakinObj = blockHolders[i].gameObject;
                }
            }

        }
        if (enYakinObj != null)
        {
            StartCoroutine(haulJob(enYakinObj));
        }
        return enYakinObj;

    }
    public GameObject lookForTree()
    {

        TreeCode[] TreeCodes = FindObjectsOfType<TreeCode>();
        GameObject enYakinObj = null;

        float distanceMin = Mathf.Infinity;
        for (int i = 0; i < TreeCodes.Length; i++)
        {
            if (TreeCodes[i].someoneOnIt == false)
            {
                float distance = myVillagerMove.CalculatePathDistance(TreeCodes[i].transform.position);
                //Debug.Log("Distance between " + this.name + " and " + blockHolders[i].name + " is " + distance + " units.");
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    enYakinObj = TreeCodes[i].gameObject;
                }
            }



        }
        if (enYakinObj != null)
        {
            StartCoroutine(lumberJackJob(enYakinObj));
        }
        return enYakinObj;

    }
    public GameObject lookForFarm()
    {

        FarmPile[] FarmCodes = FindObjectsOfType<FarmPile>();
        GameObject enYakinObj = null;

        float distanceMin = Mathf.Infinity;
        for (int i = 0; i < FarmCodes.Length; i++)
        {
            if (FarmCodes[i].someoneOnIt == false && FarmCodes[i].planted == false)
            {

                float distance = myVillagerMove.CalculatePathDistance(FarmCodes[i].transform.position);
                //Debug.Log("Distance between " + this.name + " and " + blockHolders[i].name + " is " + distance + " units.");
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    enYakinObj = FarmCodes[i].gameObject;
                }
            }



        }
        if (enYakinObj != null)
        {
            StartCoroutine(GrowJob(enYakinObj));
        }
        return enYakinObj;

    }
    public GameObject lookForFarmForHarvest()
    {

        FarmPile[] FarmCodes = FindObjectsOfType<FarmPile>();
        GameObject enYakinObj = null;

        float distanceMin = Mathf.Infinity;
        for (int i = 0; i < FarmCodes.Length; i++)
        {
            if (FarmCodes[i].someoneOnIt == false && FarmCodes[i].planted == true && FarmCodes[i].readyToHarvest == true)
            {

                float distance = myVillagerMove.CalculatePathDistance(FarmCodes[i].transform.position);
                //Debug.Log("Distance between " + this.name + " and " + blockHolders[i].name + " is " + distance + " units.");
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    enYakinObj = FarmCodes[i].gameObject;
                }
            }



        }
        if (enYakinObj != null)
        {
            StartCoroutine(PlantCutJob(enYakinObj));
        }
        return enYakinObj;

    }
    IEnumerator haulJob(GameObject haulingObj)
    {

        StartCoroutine(haulJob(haulingObj, null));
        yield break;
    }

    IEnumerator haulJob(GameObject haulingObj, GameObject preHaulingObj)
    {
        doingNow = ("haulJob: " + haulingObj.GetComponent<BlockHolder>().whoIsHold.whoAmI.name.ToString());
        haulingObj.GetComponent<BlockHolder>().someoneOnIt = true;

        onDuty = true;
        //Debug.Log("haul started");

        myVillagerMove.moveToPoint(haulingObj.transform.position);//obje ye git

        //çözüm bulamadýðým hata için
        float distanceForBug=0;
        while (!myVillagerMove.anyPathRemaining())//gitmesini bekle
        {
            if(distanceForBug == myVillagerMove.CalculatePathDistance(haulingObj.transform.position))
            {
                //Debug.LogWarning("anyPathRemaining");
                myVillagerMove.moveToPoint(new Vector3(transform.position.x+1f,transform.position.y,transform.position.z));//çözüm bulamadýðým hata için
            }
            distanceForBug = myVillagerMove.CalculatePathDistance(haulingObj.transform.position);
            yield return new WaitForSeconds(0.1f);
            //((Vector3.Distance(this.transform.position, haulingObj.transform.position))<0.7f)
        }

        //Debug.Log("in position");
        yield return new WaitForSeconds(1.0f);//1sn bekle

        
        //2023_06_12
        if (preHaulingObj != null)
        {
            int leftAmount = preHaulingObj.GetComponent<BlockHolder>().whoIsHold.whoAmI.maxAmount - preHaulingObj.GetComponent<BlockHolder>().whoIsHold.amount;

            if (leftAmount >= haulingObj.GetComponent<BlockHolder>().whoIsHold.amount)
            {
                Destroy(preHaulingObj);
                haulingObj.GetComponent<BlockHolder>().whoIsHold.amount += preHaulingObj.GetComponent<BlockHolder>().whoIsHold.amount;
            }
            else
            {
                preHaulingObj.GetComponent<BlockHolder>().whoIsHold.amount = preHaulingObj.GetComponent<BlockHolder>().whoIsHold.whoAmI.maxAmount;
                haulingObj.GetComponent<BlockHolder>().whoIsHold.amount -= leftAmount;
                haulingObj.GetComponent<BlockHolder>().someoneOnIt = false;
                haulingObj = preHaulingObj;

            }

        }
        //!2023_06_12
        
        haulingObj.GetComponent<BlockHolder>().onHaul = true;//objeyi taþýnýyo haline getirdi
        myVillagerMove.haulThis(haulingObj, true);//objeyi taþýmaya baþladý




        //2023_06_12
        
        //aldýðýmdan baþka var mý diye kontrol et
        int leftAmountt = haulingObj.GetComponent<BlockHolder>().whoIsHold.whoAmI.maxAmount - haulingObj.GetComponent<BlockHolder>().whoIsHold.amount;
        if (leftAmountt > 0)
        {
            //Debug.Log("leftAmountt: "+ leftAmountt);
            float lookingRadius = 3;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, lookingRadius);
            foreach (Collider2D collider in colliders)
            {
                //Debug.Log(collider.gameObject.name);
                
                BlockHolder blockHolder = collider.gameObject.GetComponent<BlockHolder>();
                
                if (blockHolder != null)
                {
                    
                    if (blockHolder.whoIsHold.whoAmI.name == haulingObj.GetComponent<BlockHolder>().whoIsHold.whoAmI.name)
                    {
                        if (!blockHolder.onHaul && !blockHolder.someoneOnIt && !blockHolder.inPile)
                        {

                            StartCoroutine(haulJob(blockHolder.gameObject, haulingObj));
                            yield break;
                            
                        }
                    }
                    
                }
                
            }
        }
        
        //!2023_06_12


        GameObject myPile = lookForStockPile();
        myVillagerMove.moveToPoint(myPile.transform.position);//depoya git
        while (!myVillagerMove.anyPathRemaining())//gitmesini bekle
        {
            yield return null;
        }

        //Debug.Log("in pile");
        myVillagerMove.haulThis(haulingObj, false);//objeyi taþýmayý býraktý

        myPile.GetComponent<stockPileCode>().addBlock(haulingObj.GetComponent<BlockHolder>());

        haulingObj.GetComponent<BlockHolder>().inPile = true;
        haulingObj.GetComponent<BlockHolder>().onHaul = false;
        haulingObj.GetComponent<BlockHolder>().someoneOnIt = false;


        onDuty = false;
        doingNow = ("Idle");
        yield break;
    }
    IEnumerator lumberJackJob(GameObject cuttingTreeObj)
    {
        doingNow = ("lumberJackJob: " + cuttingTreeObj.name.ToString());
        cuttingTreeObj.GetComponent<TreeCode>().someoneOnIt = true;
        onDuty = true;

        myVillagerMove.moveToPoint(cuttingTreeObj.transform.position);//obje ye git

        while (!myVillagerMove.anyPathRemaining())//gitmesini bekle
        {

            yield return null;
            //((Vector3.Distance(this.transform.position, haulingObj.transform.position))<0.7f)
        }

        yield return new WaitForSeconds(1.0f);//1sn bekle


        while (cuttingTreeObj.GetComponent<TreeCode>().HP > 0)//Aðacý kesmesini bekle
        {
            yield return new WaitForSeconds(lumberJackCD);
            cuttingTreeObj.GetComponent<TreeCode>().takeDMG(lumberJackHit);
        }



        onDuty = false;
        doingNow = ("Idle");
        yield break;
    }
    IEnumerator GrowJob(GameObject plantingObj)
    {
        doingNow = ("GrowJob: " + plantingObj.name.ToString());
        onDuty = true;

        myVillagerMove.moveToPoint(plantingObj.transform.position);//obje ye git

        while (!myVillagerMove.anyPathRemaining())//gitmesini bekle
        {

            yield return null;
            //((Vector3.Distance(this.transform.position, haulingObj.transform.position))<0.7f)
        }

        yield return new WaitForSeconds(1.0f);//1sn bekle


        plantingObj.GetComponent<FarmPile>().Plant();
        
        yield return new WaitForSeconds(0.5f);//1sn bekle


        onDuty = false;


        doingNow = ("Idle");
        yield break;
    }
    IEnumerator PlantCutJob(GameObject plantCuttingObj)
    {
        doingNow = ("PlantCutJob: " + plantCuttingObj.name.ToString());
        onDuty = true;

        myVillagerMove.moveToPoint(plantCuttingObj.transform.position);//obje ye git

        while (!myVillagerMove.anyPathRemaining())//gitmesini bekle
        {

            yield return null;
            //((Vector3.Distance(this.transform.position, haulingObj.transform.position))<0.7f)
        }

        yield return new WaitForSeconds(1.0f);//1sn bekle


        plantCuttingObj.GetComponent<FarmPile>().Harvest();

        yield return new WaitForSeconds(0.5f);//1sn bekle


        onDuty = false;


        doingNow = ("Idle");
        yield break;
    }

}
