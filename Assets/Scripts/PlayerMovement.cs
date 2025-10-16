using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;          
    [SerializeField] private Rigidbody rb;             

    [Header("Move")]
    public float playerSpeed = 2f;
    public float horizontalSpeed = 3f;
    public float speedIncreaseRate = 0.5f;

    [Header("Bounds")]
    public float rightlimit = 5.5f;
    public float leftlimit = -5.5f;

    [Header("Jump")]
    public float jumpForce = 5f;
    public string jumpStateName = "Running Forward Flip"; 
    private bool isGrounded = true;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (animator == null)
        {
           
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

      
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;

       
            if (animator != null)
                animator.Play(jumpStateName, 0, 0f); 

            
        }

        
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
