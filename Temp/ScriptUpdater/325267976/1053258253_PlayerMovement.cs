using UnityEngine;

/// <summary>
/// Endless-runner style controller:
/// - Constant forward motion with optional gradual speed increase
/// - A/D or Left/Right to strafe within bounds
/// - Space to jump (only when grounded)
/// - Animator parameters: "IsGrounded" (bool) and "Jump" (trigger)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class RunnerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;     // Animator on this object or a child (for animations)
    [SerializeField] private Rigidbody rb;          // Rigidbody used for physics movement

    [Header("Move")]
    [Min(0f)] public float playerSpeed = 5f;        // Forward (Z) speed in units/second
    [Min(0f)] public float horizontalSpeed = 3f;    // Lateral (X) speed in units/second
    public float speedIncreaseRate = 0.5f;          // How fast both speeds ramp up over time (units/sec^2)
    [Tooltip("0 = no cap. If > 0, forward speed won't exceed this value.")]
    public float maxForwardSpeed = 0f;              // Optional cap for forward speed
    [Tooltip("0 = no cap. If > 0, lateral speed won't exceed this value.")]
    public float maxHorizontalSpeed = 0f;           // Optional cap for lateral speed

    [Header("Bounds")]
    public float rightlimit = 5.5f;                 // Maximum X position
    public float leftlimit  = -5.5f;                // Minimum X position

    [Header("Jump / Ground")]
    public float jumpForce = 5f;                    // Impulse applied on jump
    public Transform groundCheck;                   // Place slightly below the feet
    public float groundCheckRadius = 0.15f;         // Sphere radius for ground detection
    public LayerMask groundMask;                    // Assign to your "Ground" layer(s)

    // Runtime state
    private bool isGrounded;                        // True when standing on ground
    private float _targetX;                         // Desired X we move towards in FixedUpdate

    private void Awake()
    {
        // Cache components if not set in Inspector
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        // Rigidbody quality-of-life defaults for a runner
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true; // Prevents tipping over from collisions

        // Initialize target X to current X so we don't snap on start
        _targetX = transform.position.x;
    }

    private void Update()
    {
        // 1) Gradually increase speeds over time (frame-rate independent)
        if (speedIncreaseRate != 0f)
        {
            playerSpeed      += speedIncreaseRate * Time.deltaTime;
            horizontalSpeed  += speedIncreaseRate * Time.deltaTime;

            // Optional speed caps (if > 0)
            if (maxForwardSpeed > 0f)     playerSpeed     = Mathf.Min(playerSpeed, maxForwardSpeed);
            if (maxHorizontalSpeed > 0f)  horizontalSpeed = Mathf.Min(horizontalSpeed, maxHorizontalSpeed);
        }

        // 2) Ground check every frame for responsive jump gating & animation
        if (groundCheck)
        {
            isGrounded = Physics.CheckSphere(
                groundCheck.position,
                groundCheckRadius,
                groundMask,
                QueryTriggerInteraction.Ignore
            );
        }
        else
        {
            // Failsafe: if no groundCheck assigned, consider always grounded
            isGrounded = true;
        }

        // Update animator boolean if present
        if (animator) animator.SetBool("IsGrounded", isGrounded);

        // 3) Jump input (space) when grounded
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Zero vertical velocity for consistent jumps
            Vector3 v = rb.linearVelocity;
            v.y = 0f;
            rb.linearVelocity = v;

            // Apply upward impulse
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Trigger jump animation (or play a specific state)
            if (animator) animator.SetTrigger("Jump");
        }

        // 4) Horizontal input (read & clamp here, actual move in FixedUpdate)
        float x = transform.position.x;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  x -= horizontalSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += horizontalSpeed * Time.deltaTime;

        // Clamp within left/right limits
        _targetX = Mathf.Clamp(x, leftlimit, rightlimit);
    }

    private void FixedUpdate()
    {
        // Physics movement should happen in FixedUpdate for stable results

        // Move forward at constant (possibly ramping) speed
        Vector3 pos = rb.position;
        pos += Vector3.forward * (playerSpeed * Time.fixedDeltaTime);

        // Smoothly move towards target X at the current horizontal speed
        pos.x = Mathf.MoveTowards(pos.x, _targetX, horizontalSpeed * Time.fixedDeltaTime);

        // Apply the new position using physics-safe method
        rb.MovePosition(pos);
    }

    private void OnValidate()
    {
        // Keep limits sane in the Inspector
        if (rightlimit < leftlimit)
        {
            float tmp = rightlimit;
            rightlimit = leftlimit;
            leftlimit = tmp;
        }

        // Non-negative radii & forces
        groundCheckRadius = Mathf.Max(0f, groundCheckRadius);
        jumpForce = Mathf.Max(0f, jumpForce);

        // Non-negative speeds
        playerSpeed = Mathf.Max(0f, playerSpeed);
        horizontalSpeed = Mathf.Max(0f, horizontalSpeed);

        // Caps cannot be negative
        maxForwardSpeed = Mathf.Max(0f, maxForwardSpeed);
        maxHorizontalSpeed = Mathf.Max(0f, maxHorizontalSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check sphere
        if (groundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Visualize lateral bounds as lines
        Gizmos.color = Color.cyan;
        Vector3 p1 = new Vector3(leftlimit, 0f, transform.position.z - 2f);
        Vector3 p2 = new Vector3(leftlimit, 0f, transform.position.z + 2f);
        Vector3 p3 = new Vector3(rightlimit, 0f, transform.position.z - 2f);
        Vector3 p4 = new Vector3(rightlimit, 0f, transform.position.z + 2f);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p3, p4);
    }
}
