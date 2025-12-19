using UnityEngine;

public abstract class EntityState
{
    // Changed from 'Player' to 'Entity'
    protected Entity entity; 
    protected StateMachine stateMachine;
    protected string animBoolName;
    protected bool triggerCalled;

    // Changed from 'Player' to 'Entity'
    public EntityState(Entity entity, StateMachine stateMachine, string animBoolName)
    {
        this.entity = entity; // Changed from 'player'
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }
    public virtual void Enter()
    {
        entity.animator.SetBool(animBoolName, true); // Changed from 'player'
        triggerCalled = false;
    }
    public virtual void Update()
    {
        entity.animator.SetFloat("yVelocity", entity.rb.linearVelocity.y); // Changed from 'player'
    }
    public virtual void Exit()
    {
        entity.animator.SetBool(animBoolName, false); // Changed from 'player'
    }
    public void CallAnimationTrigger()
    {
        triggerCalled = true;
    }
}