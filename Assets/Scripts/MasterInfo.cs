using UnityEngine;
using System.Collections;
using System.Collections.Generic;   

public class MasterInfo : MonoBehaviour
{
    public static int powerCount = 0;
    [SerializeField] GameObject powerDisplay;


    void Update()
    {
        powerDisplay.GetComponent<TMPro.TMP_Text>().text = "DOPAMINE: " + powerCount;
    }
}
