using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "DodgeFromBullet",
    story: "[Boss] Dodge Away From Bullet",
    category: "Action",
    id: "c537ce6abf409ebf4bfe3f3aae899dc7"
)]
public partial class DodgeFromBulletAction : Action
{
    [SerializeReference]
    public BlackboardVariable<BossEnemy> Boss;

    protected override Status OnStart()
    {
        if (Boss == null)
        {
            Debug.LogError("BossEnemy component is missing on the Boss GameObject.");
            return Status.Failure;
        }

        if (!Boss.Value.CanDodge())
        {
            return Status.Failure;
        }

        GameObject bullet = GameObject.FindGameObjectWithTag("Bullet");
        Debug.Log("Bullet: " + bullet);

        if (bullet == null)
        {
            Debug.LogError("Bullet not found.");
            return Status.Failure;
        }

        Vector2 dodgeDirection;
        Vector2 bulletDirection = bullet.GetComponent<Rigidbody2D>().linearVelocity.normalized;
        Vector2 bossToBullet = (
            bullet.transform.position - Boss.Value.transform.position
        ).normalized;
        float dotProduct = Vector2.Dot(bulletDirection, bossToBullet);

        if (dotProduct > 0)
        {
            dodgeDirection = new Vector2(bulletDirection.y, -bulletDirection.x);
        }
        else
        {
            dodgeDirection = new Vector2(-bulletDirection.y, bulletDirection.x);
        }

        Boss.Value.Dodge(dodgeDirection);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!Boss.Value.DodgeStatus())
        { // if not dodging anymore return success
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd() { }
}
