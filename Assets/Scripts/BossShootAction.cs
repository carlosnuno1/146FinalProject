using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "bossshoot", story: "[Boss] shoots [player] with [bulletprefab] from [firepoint] every [firerate] seconds at [bulletspeed] speed", category: "Action", id: "b50c958272dffa7683d145c10330f476")]
public partial class BossShootAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Boss;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Bulletprefab;
    [SerializeReference] public BlackboardVariable<Transform> Firepoint;
    [SerializeReference] public BlackboardVariable<float> Firerate;
    [SerializeReference] public BlackboardVariable<float> Bulletspeed;

    private float fireCooldown;
    private Transform bulletPoint;

    private GameManager gameManager;
    private float lastKnownResetCount = -1;

    protected override Status OnStart()
    {
        if (Player.Value == null)
        {
            Debug.LogError("Player is not set");
            return Status.Failure;
        }
        
        if (Boss.Value == null)
        {
            Debug.LogError("Self is not set");
            return Status.Failure;
        }

        // Get GameManager reference
        gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            // Initialize base values if not already done
            gameManager.InitializeBaseValues(Firerate.Value, Bulletspeed.Value);
            
            // Apply difficulty scaling
            (float scaledFireRate, float scaledBulletSpeed) = gameManager.GetScaledValues();
            Firerate.Value = scaledFireRate;
            Bulletspeed.Value = scaledBulletSpeed;
        }
        
        fireCooldown = Firerate.Value;
        bulletPoint = Boss.Value.transform.Find("FirePoint");
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // First check if boss or player is destroyed/null
        if (Boss.Value == null || Player.Value == null)
        {
            return Status.Failure;
        }

        // Check if we need to reapply difficulty scaling
        if (gameManager != null && gameManager.resetCount != lastKnownResetCount)
        {
            lastKnownResetCount = gameManager.resetCount;
            // Apply difficulty scaling
            (float scaledFireRate, float scaledBulletSpeed) = gameManager.GetScaledValues();
            Firerate.Value = scaledFireRate;
            Bulletspeed.Value = scaledBulletSpeed;
        }

        handleShooting();
        return Status.Running;
    }

    protected override void OnEnd()
    {

    }

    void handleShooting(){
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
        }
    }

    void Shoot()
    {
        if (Bulletprefab.Value == null){
            Debug.Log("Bulletprefab is null");
            return;
        }

        if (bulletPoint == null){
            Debug.Log("bulletPoint is null");
            return;
        }

        if (Player.Value == null){
            Debug.Log("Player is null");
            return;
        }

        GameObject projectile = GameObject.Instantiate(Bulletprefab.Value, bulletPoint.position, Quaternion.identity);
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

