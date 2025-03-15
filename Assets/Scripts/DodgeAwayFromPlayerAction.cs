using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Dodge Away From Player", story: "[Boss] Dodge Away From [Player]", category: "Action", id: "493e349ca71ce4cbcdfbd85b7c93f8b4")]
public partial class DodgeAwayFromPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<BossEnemy> Boss;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    protected override Status OnStart()
    {
        if (Boss == null)
        {

            Debug.LogError("BossEnemy component is missing on the Boss GameObject.");
            return Status.Failure;
        }

        if (!Boss.Value.CanDodge()){
            return Status.Failure;
        }

        Vector2 dodgeDirection = (Boss.Value.transform.position - Player.Value.transform.position).normalized;
        Boss.Value.Dodge(dodgeDirection);

        return Status.Running;

    }

    protected override Status OnUpdate()
    {
        if (!Boss.Value.DodgeStatus()) { // if not dodging anymore return success
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

