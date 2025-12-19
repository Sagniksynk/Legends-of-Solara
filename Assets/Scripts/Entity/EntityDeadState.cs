using System;
using UnityEngine;

public class EntityDeadState : EntityState
{
    private bool isCorpse; //tracks if body has hit the ground
    public EntityDeadState(Entity entity, StateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        isCorpse = false;
    }
    public override void Update()
    {
        base.Update();
        if (isCorpse) return;
        if(entity.isGrounded && entity.rb.linearVelocity.y <= 0.1f)
        {
            BecomeCorpse();
        }
    }

    private void BecomeCorpse()
    {
       isCorpse = true;
       entity.rb.linearVelocity = Vector2.zero;
        
       if(entity.cd !=  null) entity.cd.enabled = false;
        if (entity.rb != null) entity.rb.gravityScale = 0;
    }
}
