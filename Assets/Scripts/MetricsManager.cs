using UnityEngine;


public class Metrics {
    public int Shots { get; private set; }
    public int SuccessfulShots { get; private set; }
    public int Dodges { get; private set; }
    public int SuccessfulDodges { get; private set; }
    public int Blocks { get; private set; }
    public int SuccessfulBlocks { get; private set; }

    private string _name;

    public Metrics(string name) {
        _name = name;
    }

    public void RecordShot() { Shots++; PrintStats();}
    public void RecordSuccessfulShot() { SuccessfulShots++; PrintStats();}
    public void RecordDodge() { Dodges++; }
    public void RecordSuccessfulDodge() { SuccessfulDodges++; }
    public void RecordBlock() { Blocks++; }
    public void RecordSuccessfulBlock() { SuccessfulBlocks++; }

    public void PrintStats() {
        // Debug.Log($"{_name} \n Shots: {SuccessfulShots}/{Shots}, Dodges: {SuccessfulDodges}/{Dodges}, Blocks: {SuccessfulBlocks}/{Blocks}");
    }
}

public class MetricsManager : MonoBehaviour
{
    // Singleton class
    [Header("Metrics")]
    public Metrics playerMetrics = new Metrics("Player");
    public Metrics bossMetrics = new Metrics("Boss");

    [Header("Distance Metrics")]
    public GameObject player;
    public GameObject boss;

    private float totalDistance = 0;
    private int distanceSamples = 0;
    
    public float distanceInvokeTime = 1.0f;
    public float CurrentDistance { get; private set; }
    public float AverageDistance  { get; private set; }

    public static MetricsManager instance {get; private set;}

    private void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }
    } 

    public void Start()
    {
        InvokeRepeating(nameof(RecordDistance), distanceInvokeTime, distanceInvokeTime);
    }

    private void RecordDistance() {
        if (boss && player) {
            CurrentDistance = Vector3.Distance(player.transform.position, boss.transform.position);
            totalDistance += CurrentDistance;
            distanceSamples++;
            AverageDistance = distanceSamples > 0 ? totalDistance / distanceSamples : 0;
            // Debug.Log($"CurrentDistance: {CurrentDistance} \n AverageDistance: {AverageDistance}");
        }
    }

}
