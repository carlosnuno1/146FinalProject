using TMPro;
using Unity.AppUI.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // metricsmanager
    // boss
    public GameObject boss;
    public GameObject player;

    public TextMeshProUGUI displayText;
    public GameObject displayPanel;

    public int resetCount = 0;

    // Add these variables to store difficulty settings
    private float baseFireRate;
    private float baseBulletSpeed;
    private bool initialized = false;

    void Start()
    {
        resetCount = 0;
        // Subscribe to boss defeated event
        boss.GetComponent<EnemyHealth>().OnBossDefeated += DisplayRestart;
    }

    public void InitializeBaseValues(float fireRate, float bulletSpeed)
    {
        if (!initialized)
        {
            baseFireRate = fireRate;
            baseBulletSpeed = bulletSpeed;
            initialized = true;
            Debug.Log(
                $"Initialized base values: FireRate={baseFireRate}, BulletSpeed={baseBulletSpeed}"
            );
        }
    }

    public (float fireRate, float bulletSpeed) GetScaledValues()
    {
        float scaledFireRate = baseFireRate * Mathf.Pow(0.8f, resetCount);
        float scaledBulletSpeed = baseBulletSpeed * (1f + (0.2f * resetCount));

        Debug.Log($"=== DIFFICULTY SCALING (Level {resetCount}) ===");
        Debug.Log(
            $"Fire Rate: {baseFireRate:F2} seconds -> {scaledFireRate:F2} seconds (shoots {1 / scaledFireRate:F1}x per second)"
        );
        Debug.Log(
            $"Bullet Speed: {baseBulletSpeed:F1} -> {scaledBulletSpeed:F1} ({(scaledBulletSpeed / baseBulletSpeed):P0} of base speed)"
        );
        Debug.Log($"=======================================");

        return (scaledFireRate, scaledBulletSpeed);
    }

    public void Reset()
    {
        resetCount++;
        Debug.Log($"\n=== ATTEMPT #{resetCount} STARTING ===");

        // Reset all game state
        // Reset player
        player.transform.position = new Vector3(-5, 0, 0);
        player.SetActive(true);
        player.GetComponent<PlayerHealth>().Reset();

        // Reset boss position and health
        boss.GetComponent<BossEnemy>().ResetToInitialState();
        boss.GetComponent<EnemyHealth>().Reset();

        MetricsManager.instance.Reset();

        displayPanel.SetActive(false);
    }

    public void DisplayRestart()
    {
        displayPanel.SetActive(true);
        displayText.text = $"Boss defeated!\nAttempt #{resetCount}\n";
        // display metrics
        displayText.text += MetricsManager.instance.PrintStats();
    }
}
