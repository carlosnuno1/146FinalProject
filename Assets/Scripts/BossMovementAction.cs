using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Boss Movement", story: "[Boss] Moves Away From [Player]", category: "Action", id: "c143b56b32a897157d19aeeec13d7047")]
public partial class BossMovementAction : Action
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

        Vector2 moveAwayDirection = (Boss.Value.transform.position - Player.Value.transform.position).normalized;
        
        Boss.Value.SetMoveDirection(moveAwayDirection);

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

