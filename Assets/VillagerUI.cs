using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void ShowChoosenOnUI()
    {
        VillagerStatus[] villagerStatus = FindObjectsOfType<VillagerStatus>();
        foreach(VillagerStatus vs in villagerStatus)
        {
            if (vs.amIChosen == true)
            {
                showThisOnUI(vs,vs.gameObject.GetComponent<VillagerJobPriority2D>());
            }
        }
    }
    public static void showThisOnUI(VillagerStatus myVillagerStatus, VillagerJobPriority2D myVillagerJobPriority2D)
    {
        Debug.Log(myVillagerStatus.HP + "/" + myVillagerStatus.maxHP + "\nFood: " + myVillagerStatus.Food + "/100\nMood: " + myVillagerStatus.MoodNow + "/100\n");
        Debug.Log(myVillagerJobPriority2D.myJobs[0]);
    }
}
