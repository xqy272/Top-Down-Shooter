using Enemy.StateMachine;
using UnityEngine;

namespace Enemy.Enemy_Melee
{
    public class ChaseStateMelee : EnemyState
    {
        private readonly EnemyMelee enemy;
        private float lastTimeUpdatedDestination;
        
        public ChaseStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
        {
            enemy = enemyBase as EnemyMelee;
        }

        public override void Enter()
        {
            base.Enter();
            
            enemy.Agent.speed = enemy.chaseSpeed;
            enemy.Agent.isStopped = false;
        }

        public override void Update()
        {
            base.Update();

            if (enemy.PlayerInAttackRange())
                StateMachine.ChangeState(enemy.AttackState);
            
            enemy.FaceTarget(GetNextPathPoint());
            
            if(CanUpdateDestination())
                enemy.Agent.destination = enemy.Player.transform.position;
        }

        public override void Exit()
        {
            base.Exit();
        }

        private bool CanUpdateDestination()
        {
            if (Time.time > lastTimeUpdatedDestination + 0.25f)
            {
                lastTimeUpdatedDestination = Time.time;
                return true;
            }
            return false;
        }
    }
}
