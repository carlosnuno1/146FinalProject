using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Composite = Unity.Behavior.Composite;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "SelectWeightedAction",
    story: "Selects a random weighted child and executes it",
    category: "Flow",
    id: "2838162009e57630ee3e6be1a0abc121"
)]
public partial class SelectWeightedActionSequence : Composite
{
    [SerializeReference]
    public Node Shoot;

    [SerializeReference]
    public Node Move;

    [SerializeReference]
    public Node Dodge;
    int m_RandomIndex = 0;

    Status status;
    Node selectedNode;

    /// <inheritdoc cref="OnStart" />
    protected override Status OnStart()
    {
        float shootweight = MetricsManager.instance.SHOOTWEIGHT;
        float moveweight = MetricsManager.instance.MOVEWEIGHT;
        float dodgeweight = MetricsManager.instance.DODGEWEIGHT;

        // pick a random number between 0 and the sum of the weights
        float random = UnityEngine.Random.Range(0, shootweight + moveweight + dodgeweight);
        // execute the action based on the random number
        if (random < shootweight)
        {
            status = StartNode(Shoot);
            selectedNode = Shoot;
            if (status == Status.Success || status == Status.Failure)
                return status;

            return Status.Waiting;
        }
        else if (random < shootweight + moveweight)
        {
            status = StartNode(Move);
            selectedNode = Move;
            if (status == Status.Success || status == Status.Failure)
                return status;

            return Status.Waiting;
        }
        else
        {
            status = StartNode(Dodge);
            selectedNode = Dodge;
            if (status == Status.Success || status == Status.Failure)
                return status;

            return Status.Waiting;
        }
    }

    /// <inheritdoc cref="OnUpdate" />
    protected override Status OnUpdate()
    {
        // var status = Children[m_RandomIndex].CurrentStatus;
        // check if the action is done\
        // get the next node status
        status = selectedNode.CurrentStatus;
        if (status == Status.Success || status == Status.Failure)
            return status;

        return Status.Waiting;
    }
}
