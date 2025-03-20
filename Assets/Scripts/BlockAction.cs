using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Block", story: "[BossEnemy] begins blocking", category: "Action", id: "bc4c307b7e197346842fbf7d8a41cc93")]
public partial class BlockAction : Action
{
    [SerializeReference] public BlackboardVariable<BossEnemy> BossEnemy;

    protected override Status OnStart()
    {
        if (BossEnemy == null)
        {
            Debug.LogError("BossEnemy component is missing on the Boss GameObject.");
            return Status.Failure;
        }

        if(!BossEnemy.Value.CanBlock()){
            Debug.Log("Cooldown not ready:" + BossEnemy.Value.shieldDuration);
            return Status.Failure;
        }

        BossEnemy.Value.Block();
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Debug.Log("AreBullets: " + PlayerBulletManager.Instance.BulletCount());

        if (!PlayerBulletManager.Instance.BulletCount()) // Since BulletCount is a boolean
        {
            if (BossEnemy.Value.ShieldStatus()) // Only stop if actually shielding
            {
                //BossEnemy.Value.StopBlock();
                Debug.Log("Block ended due to no bullets.");
            }
            return Status.Success; // Ensure action exits cleanly
        }

        if(BossEnemy.Value.shieldTimer <= 0){
            Debug.Log("Block ended due to timer.");
            return Status.Success;
        }

        return Status.Running; // Continue blocking while bullets exist
    }

    protected override void OnEnd()
    {
        //BossEnemy.Value.StopBlock();
    }
}

