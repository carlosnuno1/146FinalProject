using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(
    name: "CanDodge",
    story: "[Boss] Can Dodge",
    category: "Conditions",
    id: "554078dfb3033263ccc4c1cd884861d4"
)]
public partial class CanDodgeCondition : Condition
{
    [SerializeReference]
    public BlackboardVariable<BossEnemy> Boss;

    public override bool IsTrue()
    {
        if (Boss == null)
        {
            Debug.LogError("BossEnemy component is missing on the Boss GameObject.");
            return false;
        }
        if (Boss.Value.CanDodge())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnStart() { }

    public override void OnEnd() { }
}
