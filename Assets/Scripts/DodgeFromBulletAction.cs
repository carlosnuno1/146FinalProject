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

        Debug.Log("dotProduct: " + dotProduct);

        if (dotProduct < 0)
        {
            Vector2 repulsionForce = Boss.Value.CalculateRepulsionForce(1, 0.6f);
            Debug.Log(repulsionForce);

            // Compute two perpendicular dodge directions
            Vector2 perpendicular1 = (
                new Vector2(bulletDirection.y, -bulletDirection.x) + repulsionForce
            ).normalized;
            Vector2 perpendicular2 = (
                new Vector2(-bulletDirection.y, bulletDirection.x) + repulsionForce
            ).normalized;

            Vector2 predictedPosition1 =
                (Vector2)Boss.Value.transform.position
                + (Boss.Value.dodgeDuration * Boss.Value.dodgeForce * perpendicular1);
            Vector2 predictedPosition2 =
                (Vector2)Boss.Value.transform.position
                + (Boss.Value.dodgeDuration * Boss.Value.dodgeForce * perpendicular2);

            bool outOfBoundsRight =
                predictedPosition1.x < -Boss.Value.screenBounds.x
                || predictedPosition1.x > Boss.Value.screenBounds.x
                || predictedPosition1.y < -Boss.Value.screenBounds.y
                || predictedPosition1.y > Boss.Value.screenBounds.y;
            bool outOfBoundsLeft =
                predictedPosition2.x < -Boss.Value.screenBounds.x
                || predictedPosition2.x > Boss.Value.screenBounds.x
                || predictedPosition2.y < -Boss.Value.screenBounds.y
                || predictedPosition2.y > Boss.Value.screenBounds.y;

            // Check if dodging would push the boss out of bounds
            if (outOfBoundsLeft && outOfBoundsRight)
            {
                Debug.Log("Both dodge directions would push the boss out of bounds.");
                return Status.Failure;
            }
            else if (outOfBoundsLeft && !outOfBoundsRight)
            {
                Debug.Log("Dodge direction 1 would push the boss out of bounds.");
                dodgeDirection = perpendicular1;
            }
            else if (outOfBoundsRight && !outOfBoundsLeft)
            {
                Debug.Log("Dodge direction 2 would push the boss out of bounds.");
                dodgeDirection = perpendicular2;
            }
            else
            {
                Debug.Log("Dodge directions are within bounds.");
                // Randomly choose one of the two perpendicular dodge directions
                dodgeDirection = UnityEngine.Random.value > 0.5f ? perpendicular1 : perpendicular2;
            }

            Boss.Value.Dodge(dodgeDirection);
        }
        else
        {
            Debug.Log("Bullet is behind the boss.");
        }

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
