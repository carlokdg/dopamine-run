using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;          // assegna qui l'Animator giusto!
    [SerializeField] private Rigidbody rb;               // assegna o lascialo auto-find

    [Header("Move")]
    public float playerSpeed = 2f;
    public float horizontalSpeed = 3f;
    public float speedIncreaseRate = 0.5f;

    [Header("Bounds")]
    public float rightlimit = 5.5f;
    public float leftlimit = -5.5f;

    [Header("Jump")]
    public float jumpForce = 5f;
    public string jumpStateName = "Running Forward Flip"; // deve combaciare col nome nello State
    private bool isGrounded = true;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (animator == null)
        {
            // prova a recuperarlo dal figlio (spesso il modello è figlio del player)
            animator = GetComponentInChildren<Animator>();
        }
        if (animator == null)
            Debug.LogError("[PlayerMovement] Animator mancante: assegnalo nell’Inspector.");
        else if (animator.runtimeAnimatorController == null)
            Debug.LogError("[PlayerMovement] Nessun Animator Controller assegnato all’Animator.");
    }

    void Update()
    {
        playerSpeed += speedIncreaseRate * Time.deltaTime;
        horizontalSpeed += speedIncreaseRate * Time.deltaTime;

        transform.Translate(Vector3.forward * (playerSpeed * Time.deltaTime), Space.World);

        float x = transform.position.x;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  x -= horizontalSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += horizontalSpeed * Time.deltaTime;
        x = Mathf.Clamp(x, leftlimit, rightlimit);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

        // SALTO
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;

            // Opzione A: usa direttamente lo stato (assicurati che esista e sia nel Base Layer)
            if (animator != null)
                animator.Play(jumpStateName, 0, 0f); // layer 0, riavvia a time 0

            // Opzione B (consigliata): usa un Trigger "Jump" e transizioni nell'Animator
            // if (animator != null) animator.SetTrigger("Jump");
        }

        // Tasto di test per forzare l’animazione anche senza salto (debug)
        if (Input.GetKeyDown(KeyCode.J) && animator != null)
            animator.Play(jumpStateName, 0, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
