using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Basic Movement")]
    public float moveSpeed = 5f;
    
    [Header("Dodge Settings")]
    public float dodgeForce = 15f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 1f;
    public KeyCode dodgeKey = KeyCode.Space;

    private Rigidbody2D rb;
    private float vertical;
    private float horizontal;
    private bool canDodge = true;
    private bool isDodging = false;
    private float dodgeTimer = 0f;
    private float cooldownTimer = 0f;
    private Vector2 lastMoveDirection;

    [Header("Shield Settings")]
    public GameObject shieldObject;
    public KeyCode shieldKey = KeyCode.E;
    public float shieldDuration = 2f;
    private bool canShield = true;
    private bool isShielding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input
        horizontal = Input.GetAxisRaw("Horizontal");  
        vertical = Input.GetAxisRaw("Vertical");

        // Store last move direction when moving
        if (horizontal != 0 || vertical != 0)
        {
            lastMoveDirection = new Vector2(horizontal, vertical).normalized;
        }

        // Handle dodge input
        if (Input.GetKeyDown(dodgeKey) && canDodge && !isDodging && !isShielding)
        {
            StartDodge();
        }

        if (Input.GetKey(shieldKey) && canShield){
            shieldObject.SetActive(true);
            shieldDuration -= Time.deltaTime;
            isShielding = true;
        } else {
            shieldObject.SetActive(false);
            if (shieldDuration < 2) {
                shieldDuration += Time.deltaTime;
            }
            isShielding = false;
        }

        if (shieldDuration >=  2){
            canShield = true;
        } else if (shieldDuration <= 0){
            canShield = false;
        }

        // Update timers
        UpdateDodgeState();
    }


    void FixedUpdate()
    {
        // Apply movement
        if (!isDodging)
        {
            rb.linearVelocity = new Vector2(horizontal * moveSpeed, vertical * moveSpeed);
        }
        if (isShielding){
            rb.linearVelocity = new Vector2(0, 0);
        }
    }

    void StartDodge()
    {
        if (lastMoveDirection != Vector2.zero)
        {
            // Record dodge attempt metric
            MetricsManager.instance.playerMetrics.RecordDodge();

            isDodging = true;
            canDodge = false;
            dodgeTimer = dodgeDuration;
            
            // Apply dodge force
            rb.linearVelocity = lastMoveDirection * dodgeForce;
        }
    }

    void UpdateDodgeState()
    {
        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0)
            {
                isDodging = false;
                cooldownTimer = dodgeCooldown;
            }
        }
        else if (!canDodge)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                canDodge = true;
            }
        }
    }
}
