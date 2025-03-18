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

        Vector2 dodgeDirection;
        dodgeDirection = (
            Boss.Value.transform.position - Player.Value.transform.position
        ).normalized;

        Debug.Log("Dodge direction: " + dodgeDirection);

        // Check if dodging would push the boss out of bounds
        Vector2 adjustedDodgeDirection = dodgeDirection;

        Vector2 predictedPosition =
            (Vector2)Boss.Value.transform.position
            + (dodgeDirection * Boss.Value.dodgeForce * Boss.Value.dodgeDuration);

        // float positionX = bossPosition.x + dodgeDirection.x * Boss.Value.dodgeForce;
        // float positionY = bossPosition.y + dodgeDirection.y * Boss.Value.dodgeForce;

        Debug.Log(
            $"Position X: {predictedPosition.x}, Position Y: {predictedPosition.y}, Screen Bounds: {Boss.Value.screenBounds}"
        );

        if (predictedPosition.x > Boss.Value.screenBounds.x)
        {
            adjustedDodgeDirection.x = -Mathf.Abs(dodgeDirection.x); // Dodge left
        }
        else if (predictedPosition.x < -Boss.Value.screenBounds.x)
        {
            adjustedDodgeDirection.x = Mathf.Abs(dodgeDirection.x); // Dodge right
        }

        if (predictedPosition.y > Boss.Value.screenBounds.y)
        {
            adjustedDodgeDirection.y = -Mathf.Abs(dodgeDirection.y); // Dodge down
        }
        else if (predictedPosition.y < -Boss.Value.screenBounds.y)
        {
            adjustedDodgeDirection.y = Mathf.Abs(dodgeDirection.y); // Dodge up
        }

        Debug.Log(adjustedDodgeDirection);

        Boss.Value.Dodge(adjustedDodgeDirection);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!Boss.Value.DodgeStatus())
        { // if not dodging anymore return success
            Debug.Log(Boss.Value.transform.position);
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd() { }
}
