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
    private Rigidbody2D rb;
    private float moveTimer;
    
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
    private Vector2 lastDodgeDirection = Vector2.zero;

    private Transform player;
    private float fireCooldown;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fireCooldown = fireRate; // Start at max cooldown
    }

    void Update()
    {
        HandleMovement();
        // HandleShooting();
        UpdateDodgeState();
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    void HandleMovement()
    {
        if(Vector2.Distance(transform.position, player.position) < 5f)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void HandleShooting()
    {
        if (player == null) return; // Prevent errors if player is destroyed

        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = fireRate; // Reset cooldown
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || player == null) return;

        // records metric
        MetricsManager.instance.bossMetrics.RecordShot();

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        // Get the projectile script and set direction
        EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection((player.position - firePoint.position).normalized);
        }
    }

    public bool CanDodge(){
        if (player == null || !canDodge || isDodging) return false;
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

    void OnDrawGizmosSelected()
    {
        // Visualize movement range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(startPosition, new Vector3(moveRangeX * 2, moveRangeY * 2, 0));
    }
}
