using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Boolean",
    story: "Return [Bool]",
    category: "Action",
    id: "ac23d03277de5c68e4be1be78656d6f2"
)]
public partial class BooleanAction : Action
{
    [SerializeReference]
    public BlackboardVariable<bool> Bool;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Bool)
        {
            return Status.Success;
        }
        else
        {
            return Status.Failure;
        }
    }

    protected override void OnEnd() { }
}
