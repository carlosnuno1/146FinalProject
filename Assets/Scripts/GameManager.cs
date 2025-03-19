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

    void Start()
    {
        // Subscribe to boss defeated event
        boss.GetComponent<EnemyHealth>().OnBossDefeated += DisplayRestart;
    }

    public void Reset()
    {
        // Reset all game state
        // Reset player
        player.transform.position = new Vector3(-5, 0, 0);
        player.SetActive(true);
        player.GetComponent<PlayerHealth>().Reset();

        // Reset boss position and health
        boss.GetComponent<BossEnemy>().ResetToInitialState();
        boss.GetComponent<EnemyHealth>().Reset();

        displayPanel.SetActive(false);
    }

    public void DisplayRestart()
    {
        displayPanel.SetActive(true);

        displayText.text = "Boss defeated!\n";

        // display metrics
        displayText.text += MetricsManager.instance.PrintStats();
    }
}
