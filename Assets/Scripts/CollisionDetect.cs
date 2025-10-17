using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  

public class CollisionDetect : MonoBehaviour
{


    public ParticleSystem explosionParticle;
    [SerializeField] GameObject thePlayer;

    [SerializeField] GameObject playerAnim;
    [SerializeField] AudioSource collisionFX;
    [SerializeField] AudioSource GameOverVoice;
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
        explosionParticle.Play();
        yield return new WaitForSeconds(1);
        GameOverVoice.Play();
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(0);

    }

}