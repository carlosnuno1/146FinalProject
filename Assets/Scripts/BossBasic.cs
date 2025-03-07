using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float moveRangeX = 5f;
    public float moveRangeY = 3f;
    public float hoverSpeed = 2f;

    private Vector2 startPosition;
    private Rigidbody2D rb;
    private float moveTimer;
    
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f; // Seconds between shots
    public float projectileSpeed = 5f;

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
        HandleShooting();
    }

    void HandleMovement()
    {
        // Move in a pattern (Hovering left and right)
        float newX = startPosition.x + Mathf.Sin(Time.time * hoverSpeed) * moveRangeX;
        float newY = startPosition.y + Mathf.Cos(Time.time * (hoverSpeed / 2)) * moveRangeY;

        rb.MovePosition(new Vector2(newX, newY));
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

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        // Get the projectile script and set direction
        EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection((player.position - firePoint.position).normalized);
        }
    }


    void OnDrawGizmosSelected()
    {
        // Visualize movement range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(startPosition, new Vector3(moveRangeX * 2, moveRangeY * 2, 0));
    }
}
