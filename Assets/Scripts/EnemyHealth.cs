using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        MetricsManager.instance.bossMetrics.RecordHealth(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage! Current health: {currentHealth}");

        // records metric
        MetricsManager.instance.playerMetrics.RecordSuccessfulShot();
        MetricsManager.instance.bossMetrics.RecordHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    void Die()
    {
        Debug.Log("Enemy defeated!");
        Destroy(gameObject); // Removes enemy from scene
    }
}
