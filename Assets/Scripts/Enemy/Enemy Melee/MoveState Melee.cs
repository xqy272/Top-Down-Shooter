using Enemy.StateMachine;
using UnityEngine;

namespace Enemy.Enemy_Melee
{
    public class MoveStateMelee : EnemyState
    {
        private readonly EnemyMelee enemy;
        private Vector3 _destination;
        public MoveStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
        {
            enemy = enemyBase as EnemyMelee;
        }

        public override void Enter()
        {
            base.Enter();
            
            enemy.Agent.speed = enemy.walkSpeed;
            
            _destination = enemy.GetPatrolDestination();
            enemy.Agent.SetDestination(_destination);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Update()
        {
            base.Update();
            
            enemy.FaceTarget(GetNextPathPoint());
            
            if(enemy.Agent.remainingDistance <= enemy.Agent.stoppingDistance + 0.5f)
                StateMachine.ChangeState(enemy.IdleState);
        }
    }
}
