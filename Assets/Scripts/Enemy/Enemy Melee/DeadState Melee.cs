using Enemy.StateMachine;

namespace Enemy.Enemy_Melee
{
    public class DeadStateMelee : EnemyState
    {
        private readonly EnemyMelee enemy;
        private readonly EnemyRagdoll ragdoll;
        
        // private bool interactionDisabled;
        
        public DeadStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
        {
            enemy = enemyBase as EnemyMelee;
            if (enemy) ragdoll = enemy.GetComponent<EnemyRagdoll>();
        }

        public override void Enter()
        {
            base.Enter();
            
            // interactionDisabled = false;
            
            enemy.Animator.enabled = false;
            enemy.Agent.isStopped = true;
            
            ragdoll.RagdollActive(true);
        }

        public override void Update()
        {
            base.Update();

            // if (StateTime < 0 && !interactionDisabled)
            // {
            //     ragdoll.RagdollActive(false);
            //     ragdoll.ColliderActive(false);
            //     interactionDisabled = true;
            // }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
