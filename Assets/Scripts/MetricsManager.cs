using UnityEngine;
using Unity.Behavior;
using Unity.VisualScripting;


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

    private void UpdateBlackBoard(){
        MetricsManager.instance.UpdateBlackBoard();
    }


    public void RecordShot() { Shots++; UpdateBlackBoard();}
    public void RecordSuccessfulShot() { SuccessfulShots++; UpdateBlackBoard();}
    public void RecordDodge() { Dodges++; UpdateBlackBoard();}
    public void RecordSuccessfulDodge() { SuccessfulDodges++; UpdateBlackBoard(); }
    public void RecordBlock() { Blocks++; UpdateBlackBoard();}
    public void RecordSuccessfulBlock() { SuccessfulBlocks++; UpdateBlackBoard(); }

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

    [SerializeField] private BehaviorGraphAgent behaviorGraphAgent;

    private void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }
        behaviorGraphAgent = boss.GetComponent<BehaviorGraphAgent>();
        Debug.Log(behaviorGraphAgent);
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
            UpdateBlackBoard();
        }
    }
    
    // https://docs.unity3d.com/Packages/com.unity.behavior@1.0/manual/blackboard-variables.html
    // https://docs.unity3d.com/Packages/com.unity.behavior@1.0/api/Unity.Behavior.BlackboardReference.html
    public void UpdateBlackBoard(){
        if (behaviorGraphAgent == null) return;

        var blackboard = behaviorGraphAgent.BlackboardReference;

        void SetOrAddVariable<T>(string key, T value)
        {
            if (!blackboard.SetVariableValue(key, value))
            {

                blackboard.AddVariable(key, value);
            }
        }

        SetOrAddVariable("PlayerShots", playerMetrics.Shots);
        SetOrAddVariable("PlayerSuccessfulShots", playerMetrics.SuccessfulShots);
        SetOrAddVariable("PlayerDodges", playerMetrics.Dodges);
        SetOrAddVariable("PlayerSuccessfulDodges", playerMetrics.SuccessfulDodges);
        SetOrAddVariable("PlayerBlocks", playerMetrics.Blocks);
        SetOrAddVariable("PlayerSuccessfulBlocks", playerMetrics.SuccessfulBlocks);
        
        SetOrAddVariable("BossShots", bossMetrics.Shots);
        SetOrAddVariable("BossSuccessfulShots", bossMetrics.SuccessfulShots);
        SetOrAddVariable("BossDodges", bossMetrics.Dodges);
        SetOrAddVariable("BossSuccessfulDodges", bossMetrics.SuccessfulDodges);
        SetOrAddVariable("BossBlocks", bossMetrics.Blocks);
        SetOrAddVariable("BossSuccessfulBlocks", bossMetrics.SuccessfulBlocks);

        SetOrAddVariable("CurrentDistance", CurrentDistance);
        SetOrAddVariable("AverageDistance", AverageDistance);

    }


}
