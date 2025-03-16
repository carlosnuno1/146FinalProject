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
        fireCooldown = Firerate.Value;
        bulletPoint = Boss.Value.transform.Find("FirePoint");
        
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
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
            projectileScript.SetDirection((Player.Value.transform.position - bulletPoint.position).normalized);
        }
    }
}

