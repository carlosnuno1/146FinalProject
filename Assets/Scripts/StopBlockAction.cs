using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StopBlock", story: "[BossEnemy] stops blocking", category: "Action", id: "5abc596b06c01f144d173f1867bb8ce2")]
public partial class StopBlockAction : Action
{
    [SerializeReference] public BlackboardVariable<BossEnemy> BossEnemy;

    protected override Status OnStart()
    {
        if (BossEnemy == null || BossEnemy.Value == null)
        {
            Debug.LogError("BossEnemy component is missing on the Boss GameObject.");
            return Status.Failure;
        }

        BossEnemy.Value.StopBlock();
        
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
        BossEnemy.Value.StopBlock();
        //Debug.Log("Stop blocking");
    }
}

