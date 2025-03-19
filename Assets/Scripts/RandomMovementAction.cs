using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RandomMovement", story: "[BossEnemy] Randomly Moves", category: "Action", id: "d24d46b988dff59d6ec5d7cfef00b303")]
public partial class RandomMovementAction : Action
{
    [SerializeReference] public BlackboardVariable<BossEnemy> BossEnemy;

    private Transform background;
    private Vector2 pos;
    private Vector2 scale;
    private Vector2 randomPos;
    protected override Status OnStart()
    {
        background = GameObject.Find("background").transform;
        pos = background.position;
        scale = background.localScale;

        randomPos = new Vector2(pos.x + UnityEngine.Random.Range(-scale.x * 0.5f + 1f, scale.x * 0.5f - 1f),
                                pos.y + UnityEngine.Random.Range(-scale.y * 0.5f + 1f, scale.y * 0.5f - 1f));
        
        BossEnemy.Value.randomMove = true;
        BossEnemy.Value.SetMovePosition(randomPos);

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

