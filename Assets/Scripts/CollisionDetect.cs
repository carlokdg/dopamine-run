using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionDetect : MonoBehaviour
{


    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject playerAnim;
    [SerializeField] AudioSource collisionFX;
    [SerializeField] GameObject mainCam;
    [SerializeField] GameObject fadeOut;
    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(CollisionEnd());
    }
    
    IEnumerator CollisionEnd()
    {
        collisionFX.Play();
        //Disable Player Movement Script
        thePlayer.GetComponent<PlayerMovement>().enabled = false;
        playerAnim.GetComponent<Animator>().Play("Stumble Backwards");
        mainCam.GetComponent<Animator>().Play("CollisionCam");
        yield return new WaitForSeconds(1);
        fadeOut.SetActive(true);

    }

}