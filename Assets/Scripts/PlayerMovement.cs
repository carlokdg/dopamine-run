using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] AudioSource powerFX;

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

    [Header("Immune / Fly")]
    public float immuneDuration = 5f;

    public enum FlyMode { GlideNoGravity, BoostedJump }
    public FlyMode flyMode = FlyMode.GlideNoGravity;

    public float glideRiseHeight = 3f;
    public float glideRiseTime = 0.6f;
    public float forwardBoostWhileFlying = 1.5f;
    public float boostedJumpForce = 10f;

    [Header("Animation States")]
    public string runStateName = "Running";
    public string flyStateName = "Flying";

    [Header("Animation Timing")]
    public float flyDuration = 5f;

    private bool isImmune = false;
    private bool isFlying = false;
    private float basePlayerSpeed;
    private Coroutine flyCo;
    private float prevAnimatorSpeed = 1f;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        basePlayerSpeed = playerSpeed;
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

        if (!isFlying && isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            if (animator != null) animator.Play(jumpStateName, 0, 0f);
        }

        if (Input.GetKeyDown(KeyCode.J) && animator != null)
            animator.Play(jumpStateName, 0, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            if (!isFlying && animator != null && !string.IsNullOrEmpty(runStateName))
                animator.CrossFadeInFixedTime(runStateName, 0.1f, 0, 0f);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ImmuneCoin"))
        {
            Destroy(other.gameObject);
            if (!isImmune)
            {
                if (flyCo != null) StopCoroutine(flyCo);
                flyCo = StartCoroutine(FlyRoutine());
            }
        }
    }

    private IEnumerator FlyRoutine()
    {
        isImmune = true;
        isFlying = true;

        if (animator != null && !string.IsNullOrEmpty(flyStateName))
            animator.CrossFadeInFixedTime(flyStateName, 0.1f, 0, 0f);


             powerFX.Play();

        bool prevUseGravity = rb.useGravity;
        float savedBaseSpeed = basePlayerSpeed;
        basePlayerSpeed = playerSpeed;
        playerSpeed += forwardBoostWhileFlying;

        float duration = Mathf.Max(0.01f, flyDuration);
        float clipLen = GetClipLength(animator, flyStateName);
        prevAnimatorSpeed = animator != null ? animator.speed : 1f;
        if (animator != null)
            animator.speed = clipLen > 0f ? (clipLen / duration) : 1f;

        if (flyMode == FlyMode.GlideNoGravity)
        {
            rb.useGravity = false;
            float rise = Mathf.Min(glideRiseTime, duration);
            float startY = transform.position.y;
            float targetY = startY + Mathf.Max(0.1f, glideRiseHeight);
            float t = 0f;
            while (t < rise)
            {
                t += Time.deltaTime;
                float y = Mathf.Lerp(startY, targetY, Mathf.Clamp01(t / Mathf.Max(0.0001f, rise)));
                Vector3 p = transform.position; p.y = y;
                transform.position = p;
                yield return null;
            }
            float timer = 0f;
            float remain = Mathf.Max(0f, duration - rise);
            while (timer < remain)
            {
                Vector3 p = transform.position;
                p.y = Mathf.Lerp(p.y, targetY, 0.08f);
                transform.position = p;
                timer += Time.deltaTime;
                yield return null;
            }
            rb.useGravity = prevUseGravity;
        }
        else
        {
            rb.AddForce(Vector3.up * boostedJumpForce, ForceMode.Impulse);
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        if (animator != null) animator.speed = prevAnimatorSpeed;
        playerSpeed = basePlayerSpeed;
        basePlayerSpeed = savedBaseSpeed;
        isFlying = false;
        isImmune = false;

        if (animator != null && !string.IsNullOrEmpty(runStateName))
            animator.CrossFadeInFixedTime(runStateName, 0.1f, 0, 0f);
    }

    private float GetClipLength(Animator anim, string stateName)
    {
        if (anim == null || anim.runtimeAnimatorController == null || string.IsNullOrEmpty(stateName)) return 0f;
        var clips = anim.runtimeAnimatorController.animationClips;
        if (clips == null || clips.Length == 0) return 0f;
        var clip = clips.FirstOrDefault(c => c != null && c.name == stateName);
        return clip != null ? clip.length : 0f;
    }
}
