using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(
    name: "CanBlock",
    story: "Boss Can Block",
    category: "Conditions",
    id: "39b01447a2306766ad0100a55efe3eb5"
)]
public partial class CanBlockCondition : Condition
{
    public override bool IsTrue()
    {
        BossEnemy boss = GameObject.FindAnyObjectByType<BossEnemy>();
        return boss.CanBlock();
    }

    public override void OnStart() { }

    public override void OnEnd() { }
}
