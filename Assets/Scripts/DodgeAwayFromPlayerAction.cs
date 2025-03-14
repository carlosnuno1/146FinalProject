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
            Debug.Log("Boss", Boss.Value);
            Debug.LogError("BossEnemy component is missing on the Boss GameObject.");
            return Status.Failure;
        }
         Debug.Log("Boss", Boss.Value);

        if (!Boss.Value.CanDodge()){
            return Status.Failure;
        }

        Vector2 dodgeDirection = (Boss.Value.transform.position - Player.Value.transform.position).normalized;
        Boss.Value.Dodge(dodgeDirection);

        return Status.Running;

    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

