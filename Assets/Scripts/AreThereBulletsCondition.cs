using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(
    name: "AreThereBullets",
    story: "Player Is Shooting",
    category: "Conditions",
    id: "25e3e6bb490009b9641c09a07e1f36e2"
)]
public partial class AreThereBulletsCondition : Condition
{
    public override bool IsTrue()
    {
        if (PlayerBulletManager.Instance.BulletCount())
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
