using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;      // assegnalo nell’Inspector
    [SerializeField] private Rigidbody rb;

    [Header("Speeds")]
    public float playerSpeed = 2f;
    public float horizontalSpeed = 3f;
    public float speedIncreaseRate = 0.5f;

    [Header("Bounds")]
    public float rightlimit = 5.5f;
    public float leftlimit = -5.5f;

    [Header("Jump")]
    public float jumpForce = 5f;
    private bool isGrounded = true;

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Aumento progressivo velocità (se lo vuoi davvero su entrambi gli assi)
        playerSpeed += speedIncreaseRate * Time.deltaTime;
        horizontalSpeed += speedIncreaseRate * Time.deltaTime;

        // Movimento avanti costante
        transform.Translate(Vector3.forward * (playerSpeed * Time.deltaTime), Space.World);

        // Input laterale
        float x = transform.position.x;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            x -= horizontalSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            x += horizontalSpeed * Time.deltaTime;

        // Limiti laterali
        x = Mathf.Clamp(x, leftlimit, rightlimit);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

        // Salto: SOLO se a terra
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;

            // Fai partire il trigger di salto (meglio di .Play su uno stato specifico)
            if (animator != null)
                animator.SetTrigger("Jump");
        }

        // (Opzionale) Passa la velocità all'animator per blend di corsa
        if (animator != null)
            animator.SetFloat("Speed", rb.linearVelocity.magnitude);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ATTENZIONE: ora le graffe racchiudono TUTTO ciò che deve succedere solo quando tocchi il terreno
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            // (Opzionale) Se usi un bool "IsGrounded" nell'Animator:
            if (animator != null)
                animator.SetBool("IsGrounded", true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;

            if (animator != null)
                animator.SetBool("IsGrounded", false);
        }
    }
}
