using UnityEngine;
using System.Collections;

public class CollectPowerUpss : MonoBehaviour

{
    [SerializeField] AudioSource powerFX;
    void OnTriggerEnter(Collider other)
        {
        powerFX.Play();

        //Reference MasterInfo Script
        MasterInfo.powerCount += 1;

        this.gameObject.SetActive(false);
    }

}