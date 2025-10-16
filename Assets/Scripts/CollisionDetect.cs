using UnityEngine;

public class CollisionDetect : MonoBehaviour
{


    [SerializeField] GameObject thePlayer;
    [SerializeField] GameObject playerAnim;
     void OnTriggerEnter(Collider other)
    {
        //Disable Player Movement Script
        thePlayer.GetComponent<PlayerMovement>().enabled = false;
        playerAnim.GetComponent<Animator>().Play("Stumble Backwards");
    }

}