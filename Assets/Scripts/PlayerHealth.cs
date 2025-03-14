using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        MetricsManager.instance.playerMetrics.RecordHealth(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        MetricsManager.instance.playerMetrics.RecordHealth(currentHealth);
        Debug.Log($"Player took {damage} damage! Current health: {currentHealth}");
       
        // records metric
        MetricsManager.instance.bossMetrics.RecordSuccessfulShot();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        MetricsManager.instance.playerMetrics.RecordHealth(currentHealth);
        Debug.Log($"Player healed {amount} health! Current health: {currentHealth}");
    }

    void Die()
    {
        Debug.Log("Player has died!");
        // Add death logic (e.g., restart level, show game over screen)
        gameObject.SetActive(false);
    }
}
