using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Dodge Away From Player",
    story: "[Boss] Dodge Away From [Player]",
    category: "Action",
    id: "493e349ca71ce4cbcdfbd85b7c93f8b4"
)]
public partial class DodgeAwayFromPlayerAction : Action
{
    [SerializeReference]
    public BlackboardVariable<BossEnemy> Boss;

    [SerializeReference]
    public BlackboardVariable<GameObject> Player;

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
        Vector2 dodgeDirection;
        if (bullet != null)
        {
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
        }
        else
        {
            dodgeDirection = (
                Boss.Value.transform.position - Player.Value.transform.position
            ).normalized;
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
