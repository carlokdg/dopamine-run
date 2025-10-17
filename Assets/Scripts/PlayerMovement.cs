using UnityEngine;
using System.Collections;

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

    [Header("Immune / Fly")]
    public float immuneDuration = 5f;

    public enum FlyMode { GlideNoGravity, BoostedJump }
    [Tooltip("GlideNoGravity: gravità OFF e volo; BoostedJump: salto potenziato con gravità ON")]
    public FlyMode flyMode = FlyMode.GlideNoGravity;

    [Tooltip("Altezza da guadagnare all'inizio del volo (solo Glide)")]
    public float glideRiseHeight = 3f;

    [Tooltip("Secondi impiegati per salire all'altezza di glide (solo Glide)")]
    public float glideRiseTime = 0.6f;

    [Tooltip("Boost di velocità in avanti SOLO durante il volo (opzionale)")]
    public float forwardBoostWhileFlying = 1.5f;

    [Tooltip("Forza di super salto per la modalità BoostedJump")]
    public float boostedJumpForce = 10f;

    private bool isImmune = false;
    private bool isFlying = false;
    private float basePlayerSpeed;
    private Coroutine flyCo;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogError("[PlayerMovement] Animator mancante: assegnalo nell’Inspector.");
        else if (animator.runtimeAnimatorController == null)
            Debug.LogError("[PlayerMovement] Nessun Animator Controller assegnato all’Animator.");

        basePlayerSpeed = playerSpeed;
    }

    void Update()
    {
        // Aumenti progressivi di velocità
        playerSpeed += speedIncreaseRate * Time.deltaTime;
        horizontalSpeed += speedIncreaseRate * Time.deltaTime;

        // Avanzamento continuo in avanti
        transform.Translate(Vector3.forward * (playerSpeed * Time.deltaTime), Space.World);

        // Movimento laterale con limiti
        float x = transform.position.x;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))  x -= horizontalSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) x += horizontalSpeed * Time.deltaTime;
        x = Mathf.Clamp(x, leftlimit, rightlimit);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

        // Salto normale (solo se a terra e NON in volo)
        if (!isFlying && isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            if (animator != null) animator.Play(jumpStateName, 0, 0f);
        }

        // Tasto per testare l'animazione del salto (opzionale)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ImmuneCoin"))
        {
            Destroy(other.gameObject);
            if (!isImmune)
            {
                // Avvia l'immunità e il "volo"
                if (flyCo != null) StopCoroutine(flyCo);
                flyCo = StartCoroutine(FlyRoutine());
            }
        }
    }

    private IEnumerator FlyRoutine()
    {
        isImmune = true;
        isFlying = true;

        // Salva stato
        bool prevUseGravity = rb.useGravity;
        float savedBaseSpeed = basePlayerSpeed;
        basePlayerSpeed = playerSpeed; // fotografa la velocità attuale come base

        // Boost in avanti solo durante il volo (opzionale)
        playerSpeed += forwardBoostWhileFlying;

        if (flyMode == FlyMode.GlideNoGravity)
        {
            // 1) GLIDE: spegni gravità, sali fino a un target, poi “galleggia”
            rb.useGravity = false;

            float startY = transform.position.y;
            float targetY = startY + Mathf.Max(0.1f, glideRiseHeight);
            float t = 0f;

            // fase di salita morbida
            while (t < glideRiseTime)
            {
                t += Time.deltaTime;
                float y = Mathf.Lerp(startY, targetY, Mathf.Clamp01(t / glideRiseTime));
                Vector3 p = transform.position; p.y = y;
                transform.position = p;
                yield return null;
            }

            // fase di glide: mantieni più o meno la quota
            float timer = 0f;
            while (timer < immuneDuration - glideRiseTime)
            {
                // opzionale: piccolo smoothing per non “driftare” in Y
                Vector3 p = transform.position; 
                p.y = Mathf.Lerp(p.y, targetY, 0.08f);
                transform.position = p;

                timer += Time.deltaTime;
                yield return null;
            }

            // ripristina gravità
            rb.useGravity = prevUseGravity;
        }
        else // BoostedJump
        {
            // 2) BOOSTED JUMP: mantieni gravità ma dai una spinta forte verso l'alto
            rb.AddForce(Vector3.up * boostedJumpForce, ForceMode.Impulse);
            float timer = 0f;
            while (timer < immuneDuration)
            {
                // niente di speciale qui: la gravità farà il suo, tu continui ad avanzare per codice
                timer += Time.deltaTime;
                yield return null;
            }
        }

        // Ripristini
        playerSpeed = basePlayerSpeed;   // togli il boost in avanti
        basePlayerSpeed = savedBaseSpeed;
        isFlying = false;
        isImmune = false;
    }
}
