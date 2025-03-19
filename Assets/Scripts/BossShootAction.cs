using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "bossshoot",
    story: "[Boss] shoots [player] with [bulletprefab] from [firepoint] every [firerate] seconds at [bulletspeed] speed",
    category: "Action",
    id: "b50c958272dffa7683d145c10330f476"
)]
public partial class BossShootAction : Action
{
    [SerializeReference]
    public BlackboardVariable<GameObject> Boss;

    [SerializeReference]
    public BlackboardVariable<GameObject> Player;

    [SerializeReference]
    public BlackboardVariable<GameObject> Bulletprefab;

    [SerializeReference]
    public BlackboardVariable<Transform> Firepoint;

    [SerializeReference]
    public BlackboardVariable<float> Firerate;

    [SerializeReference]
    public BlackboardVariable<float> Bulletspeed;

    private float fireCooldown;
    private Transform bulletPoint;

    private GameManager gameManager;
    private float lastKnownResetCount = -1;

    private const int SHOTS_BEFORE_SUCCESS = 2;
    private int shotsFired = 0;

    protected override Status OnStart()
    {
        if (Player.Value == null)
        {
            Debug.LogError("Player is not set");
            return Status.Failure;
        }

        if (Boss.Value == null)
        {
            Debug.LogError("Boss is not set");
            return Status.Failure;
        }

        BossEnemy bossComponent = Boss.Value.GetComponent<BossEnemy>();
        if (bossComponent == null)
        {
            Debug.LogError("BossEnemy component not found");
            return Status.Failure;
        }

        bossComponent.Shoot();
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd() { }

    void handleShooting()
    {
        if (Player.Value == null)
        {
            Debug.LogError("Player is not set");
            return;
        }

        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = Firerate.Value;
            shotsFired++;
        }
    }

    void Shoot()
    {
        if (Bulletprefab.Value == null)
        {
            Debug.Log("Bulletprefab is null");
            return;
        }

        if (bulletPoint == null)
        {
            Debug.Log("bulletPoint is null");
            return;
        }

        if (Player.Value == null)
        {
            Debug.Log("Player is null");
            return;
        }

        GameObject projectile = GameObject.Instantiate(
            Bulletprefab.Value,
            bulletPoint.position,
            Quaternion.identity
        );
        Debug.Log("Projectile instantiated");
        EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        if (projectileScript != null)
        {
            Vector3 direction = (Player.Value.transform.position - bulletPoint.position).normalized;
            projectileScript.SetDirection(direction);
            projectileScript.speed = Bulletspeed.Value;
        }
    }
}
