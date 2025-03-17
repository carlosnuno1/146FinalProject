using Unity.Behavior;
using Unity.VisualScripting;
using UnityEngine;

public class Metrics
{
    public int Shots { get; private set; }
    public int SuccessfulShots { get; private set; }
    public int Dodges { get; private set; }
    public int SuccessfulDodges { get; private set; }
    public int Blocks { get; private set; }
    public int SuccessfulBlocks { get; private set; }

    public int Health { get; private set; }

    private string _name;

    public Metrics(string name)
    {
        _name = name;
    }

    private void UpdateBlackBoard()
    {
        MetricsManager.instance.UpdateBlackBoard();
    }

    public void RecordShot()
    {
        Shots++;
        UpdateBlackBoard();
    }

    public void RecordSuccessfulShot()
    {
        SuccessfulShots++;
        UpdateBlackBoard();
    }

    public void RecordDodge()
    {
        Dodges++;
        UpdateBlackBoard();
    }

    public void RecordSuccessfulDodge()
    {
        SuccessfulDodges++;
        UpdateBlackBoard();
    }

    public void RecordBlock()
    {
        Blocks++;
        UpdateBlackBoard();
    }

    public void RecordSuccessfulBlock()
    {
        SuccessfulBlocks++;
        UpdateBlackBoard();
    }

    public void RecordHealth(int health)
    {
        Health = health;
        UpdateBlackBoard();
    }

    public bool AreBullets => PlayerBulletManager.Instance.BulletCount();

    public void PrintStats()
    {
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
    public float AverageDistance { get; private set; }

    public static MetricsManager instance { get; private set; }

    [SerializeField]
    private BehaviorGraphAgent behaviorGraphAgent;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        behaviorGraphAgent = boss.GetComponent<BehaviorGraphAgent>();
        Debug.Log(behaviorGraphAgent);
    }

    public void Start()
    {
        InvokeRepeating(nameof(RecordDistance), distanceInvokeTime, distanceInvokeTime);
    }

    private void RecordDistance()
    {
        if (boss && player)
        {
            CurrentDistance = Vector3.Distance(player.transform.position, boss.transform.position);
            totalDistance += CurrentDistance;
            distanceSamples++;
            AverageDistance = distanceSamples > 0 ? totalDistance / distanceSamples : 0;
            // Debug.Log($"CurrentDistance: {CurrentDistance} \n AverageDistance: {AverageDistance}");
            UpdateBlackBoard();
        }
    }

    void Update()
    {
        UpdateBlackBoard(); // Updates AreBullets and other variables every frame
    }

    // https://docs.unity3d.com/Packages/com.unity.behavior@1.0/manual/blackboard-variables.html
    // https://docs.unity3d.com/Packages/com.unity.behavior@1.0/api/Unity.Behavior.BlackboardReference.html
    public void UpdateBlackBoard()
    {
        if (behaviorGraphAgent == null)
            return;

        var blackboard = behaviorGraphAgent.BlackboardReference;

        void SetOrAddVariable<T>(string key, T value)
        {
            bool x = blackboard.SetVariableValue(key, value);
            // Debug.Log($"Set {key} to {value} and its {x}");
        }

        SetOrAddVariable("PlayerShots", playerMetrics.Shots);
        SetOrAddVariable("PlayerSuccessfulShots", playerMetrics.SuccessfulShots);
        SetOrAddVariable(
            "PlayerShotAccuracy",
            playerMetrics.Shots > 0 ? (float)playerMetrics.SuccessfulShots / playerMetrics.Shots : 0
        );
        SetOrAddVariable("PlayerDodges", playerMetrics.Dodges);
        SetOrAddVariable("PlayerSuccessfulDodges", playerMetrics.SuccessfulDodges);
        SetOrAddVariable(
            "PlayerDodgeAccuracy",
            playerMetrics.Dodges > 0
                ? (float)playerMetrics.SuccessfulDodges / playerMetrics.Dodges
                : 0
        );
        SetOrAddVariable("PlayerBlocks", playerMetrics.Blocks);
        SetOrAddVariable("PlayerSuccessfulBlocks", playerMetrics.SuccessfulBlocks);
        SetOrAddVariable(
            "PlayerBlockAccuracy",
            playerMetrics.Blocks > 0
                ? (float)playerMetrics.SuccessfulBlocks / playerMetrics.Blocks
                : 0
        );

        SetOrAddVariable("BossShots", bossMetrics.Shots);
        SetOrAddVariable("BossSuccessfulShots", bossMetrics.SuccessfulShots);
        SetOrAddVariable(
            "BossShotAccuracy",
            bossMetrics.Shots > 0 ? (float)bossMetrics.SuccessfulShots / bossMetrics.Shots : 0
        );
        SetOrAddVariable("BossDodges", bossMetrics.Dodges);
        SetOrAddVariable("BossSuccessfulDodges", bossMetrics.SuccessfulDodges);
        SetOrAddVariable(
            "BossDodgeAccuracy",
            bossMetrics.Dodges > 0 ? (float)bossMetrics.SuccessfulDodges / bossMetrics.Dodges : 0
        );
        SetOrAddVariable("BossBlocks", bossMetrics.Blocks);
        SetOrAddVariable("BossSuccessfulBlocks", bossMetrics.SuccessfulBlocks);
        SetOrAddVariable(
            "BossBlockAccuracy",
            bossMetrics.Blocks > 0 ? (float)bossMetrics.SuccessfulBlocks / bossMetrics.Blocks : 0
        );

        SetOrAddVariable("CurrentDistance", CurrentDistance);
        SetOrAddVariable("AverageDistance", AverageDistance);

        SetOrAddVariable("PlayerHealth", playerMetrics.Health);
        SetOrAddVariable("BossHealth", bossMetrics.Health);

        SetOrAddVariable("AreBullets", playerMetrics.AreBullets);
    }
}
