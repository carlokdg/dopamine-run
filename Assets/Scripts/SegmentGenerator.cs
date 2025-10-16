using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewMonoBehaviourScript : MonoBehaviour


{

    public GameObject segmentMap01;
    public GameObject segmentMap02;
    public GameObject segmentMap03;
    public GameObject segmentMap04;
    public GameObject segmentMap05;
    public GameObject segmentMap06;
    public GameObject segmentMap07;
    


    void Start()
    {
        StartCoroutine(SegmentGen());
    }

    void Update()
    {

    }
    IEnumerator SegmentGen()
    {
        yield return new WaitForSeconds(5);
        segmentMap02.SetActive(true);
        yield return new WaitForSeconds(5);
        segmentMap03.SetActive(true);

    }
}
