using UnityEngine;
using System.Collections;

public class CollectPowerUpss : MonoBehaviour

{
    [SerializeField] AudioSource powerFX;
    void OnTriggerEnter(Collider other)
        {
        powerFX.Play();
        this.gameObject.SetActive(false);
    }

}