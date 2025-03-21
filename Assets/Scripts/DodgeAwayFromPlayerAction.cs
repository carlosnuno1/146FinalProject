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
        // dodgeDirection = (
        //     Boss.Value.transform.position - Player.Value.transform.position
        // ).normalized;

        Vector2 baseDodgeDirection = (
            Boss.Value.transform.position - Player.Value.transform.position
        ).normalized;

        Debug.Log("Dodge direction: " + baseDodgeDirection);

        // Generate a random angle between -90 and 90 degrees
        float randomAngle = UnityEngine.Random.Range(-45f, 45f);

        Vector2 repulsionForce = Boss.Value.CalculateRepulsionForce(1, 1.5f);

        // Rotate the base direction by the random angle
        dodgeDirection = (
            (Vector2)(Quaternion.Euler(0, 0, randomAngle) * baseDodgeDirection) + repulsionForce
        ).normalized;

        Debug.Log("Dodge direction: " + dodgeDirection);

        // Check if dodging would push the boss out of bounds
        Vector2 adjustedDodgeDirection = dodgeDirection;

        Vector2 predictedPosition =
            (Vector2)Boss.Value.transform.position
            + (Boss.Value.dodgeDuration * Boss.Value.dodgeForce * dodgeDirection);

        Boss.Value.Dodge(dodgeDirection);

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
