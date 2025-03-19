using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float moveRangeX = 5f;
    public float moveRangeY = 3f;
    public float hoverSpeed = 2f;

    private Vector2 startPosition;
    private Vector2 moveDirection;
    private Vector2 movePosition;
    private Rigidbody2D rb;

    public bool randomMove = false;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f; // Seconds between shots
    public float projectileSpeed = 5f;

    [Header("Dodging")]
    public float dodgeForce = 15f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 1f;
    private bool canDodge = true;
    private bool isDodging = false;
    private float dodgeTimer = 0f;
    private float cooldownTimer = 0f;

    public Vector2 screenBounds;

    [Header("Shield Settings")]
    public GameObject shieldObject;
    public float shieldDuration = 2f;

    private float shieldTimer;
    private bool canShield = true;
    private bool isShielding = false;

    private Transform player;
    private float fireCooldown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fireCooldown = fireRate; // Start at max cooldown
        shieldTimer = shieldDuration;

        // set screen bounds
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    private void ClampPosition()
    {
        float clampedX = Mathf.Clamp(transform.position.x, -screenBounds.x, screenBounds.x);
        float clampedY = Mathf.Clamp(transform.position.y, -screenBounds.y, screenBounds.y);

        transform.position = new Vector2(clampedX, clampedY);
    }

    public void ResetToInitialState()
    {
        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
        randomMove = false;
        canDodge = true;
        isDodging = false;
        dodgeTimer = 0f;
        cooldownTimer = 0f;
        canShield = true;
        isShielding = false;
        shieldTimer = shieldDuration;
        fireCooldown = fireRate;
        gameObject.SetActive(true);
    }

    void Update()
    {
        HandleMovement();
        // HandleShooting();
        UpdateDodgeState();
        ClampPosition();
        UpdateBlockState();
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    public void SetMovePosition(Vector2 position)
    {
        movePosition = position;
    }

    void HandleMovement()
    {
        if (isDodging || isShielding)
            return; // Don't move while dodging or blocking
        if (Vector2.Distance(transform.position, player.position) < 5f)
        {   // Move away from player
            rb.linearVelocity = moveDirection * moveSpeed;
            randomMove = false; // Interrupts random movement
        }
        else if(randomMove)
        {   // Move randomly
            if((Vector2)transform.position == movePosition)
            {
                randomMove = false;
                return; // Finished moving to the random position
            }
            moveDirection = (movePosition - (Vector2)transform.position).normalized;
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {   // No movement
            rb.linearVelocity = Vector2.zero;
        }
    }

    void HandleShooting()
    {
        if (player == null)
            return; // Prevent errors if player is destroyed

        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = fireRate; // Reset cooldown
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || player == null)
            return;

        // records metric
        MetricsManager.instance.bossMetrics.RecordShot();

        // Instantiate the projectile
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        // Get the projectile script and set direction
        EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection((player.position - firePoint.position).normalized);
        }
    }

    public Vector2 CalculateRepulsionForce(float threshold, float repulsionStrength)
    {
        Vector2 repulsionForce = Vector2.zero;

        if (transform.position.x > screenBounds.x - threshold)
            repulsionForce.x -= repulsionStrength; // Push left
        if (transform.position.x < -screenBounds.x + threshold)
            repulsionForce.x += repulsionStrength; // Push right
        if (transform.position.y > screenBounds.y - threshold)
            repulsionForce.y -= repulsionStrength; // Push down
        if (transform.position.y < -screenBounds.y + threshold)
            repulsionForce.y += repulsionStrength; // Push up

        return repulsionForce;
    }

    public bool CanDodge()
    {
        if (player == null || !canDodge || isDodging)
            return false;
        return true;
    }

    public void Dodge(Vector2 direction)
    {
        // Record dodge attempt for the boss
        MetricsManager.instance.bossMetrics.RecordDodge();

        isDodging = true;
        canDodge = false;
        dodgeTimer = dodgeDuration;

        // Apply dodge force
        rb.linearVelocity = direction * dodgeForce;
    }

    public bool DodgeStatus()
    {
        return isDodging;
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
                rb.linearVelocity = Vector2.zero; // Stop movement after dodge
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

    public bool CanBlock()
    {
        return player != null && canShield && !isShielding;
    }

    public void Block()
    {
        if (!CanBlock() || isShielding)
            return;

        isShielding = true;
        shieldObject.SetActive(true);
        //Debug.Log("Boss started blocking.");
    }

    public void UpdateBlockState()
    {
        if (isShielding)
        {
            // Decrease shield timer while active
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0)
            {
                StopBlock(); // Automatically stop if timer runs out
            }
        }
        else
        {
            // Regenerate shield when not in use
            if (shieldTimer < shieldDuration)
            {
                shieldTimer += Time.deltaTime / 4;
            }
        }
    }

    public void StopBlock()
    {
        if (!isShielding)
            return; // Prevent multiple calls

        isShielding = false;
        shieldObject.SetActive(false);

        Debug.Log("Boss stopped blocking. Remaining shield time: " + shieldTimer);
    }

    public bool ShieldStatus()
    {
        return isShielding;
    }

    void OnDrawGizmosSelected()
    {
        // Visualize movement range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(startPosition, new Vector3(moveRangeX * 2, moveRangeY * 2, 0));
    }
}
