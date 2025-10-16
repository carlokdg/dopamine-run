using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    [Header("Move")]
    public float playerSpeed = 5f;
    public float horizontalSpeed = 3f;
    public float speedIncreaseRate = 0.5f;

    [Header("Bounds")]
    public float rightlimit = 5.5f;
    public float leftlimit = -5.5f;

    [Header("Jump / Ground")]
    public float jumpForce = 5f;
    public Transform groundCheck;            // Place slightly below feet
    public float groundCheckRadius = 0.15f;
    public LayerMask groundMask;             // Assign "Ground" layer
    private bool isGrounded;

    // Internal
    private float _targetX;

    private void Awake()
    {
        // Cache components and make Rigidbody stable for character-style motion
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true; // Prevent tipping over
    }

    private void Update()
    {
        // Gradually increase speeds over time (endless runner feel)
        playerSpeed += speedIncreaseRate * Time.deltaTime;
        horizontalSpeed += speedIncreaseRate * Time.deltaTime;

        // Ground check every frame (animation + jump gating)
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
        if (animator) animator.SetBool("IsGrounded", isGrounded);

        // Jump (impulse) only when grounded
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Reset vertical velocity for consistent jump height
            Vector3 v = rb.linearVelocity;
            v.y = 0f;
            rb.linearVelocity = v;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (animator) animator.SetTrigger("Jump");
        }

        // Read lateral input here; move via physics in FixedUpdate
        float x = transform.position.x;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  x -= horizontalSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += horizontalSpeed * Time.deltaTime;

        // Clamp to lane bounds and store desired x
        _targetX = Mathf.Clamp(x, leftlimit, rightlimit);
    }

    private void FixedUpdate()
    {
        // Advance forward and slide toward targetX using Rigidbody for smooth, stable motion
        Vector3 pos = rb.position;
        pos += Vector3.forward * (playerSpeed * Time.fixedDeltaTime);
        pos.x = Mathf.MoveTowards(pos.x, _targetX, horizontalSpeed * Time.fixedDeltaTime);

        rb.MovePosition(pos);
    }

    // Visualize ground check radius in editor
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endif
}
