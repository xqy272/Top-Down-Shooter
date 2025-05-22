using Enemy.StateMachine;

namespace Enemy.Enemy_Melee
{
    public class IdleStateMelee : EnemyState
    {
        private readonly EnemyMelee enemy;
        public IdleStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
        {
            enemy = enemyBase as EnemyMelee;
        }

        public override void Enter()
        {
            base.Enter();
            
            StateTime = EnemyBase.idleTime;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void Update()
        {
            base.Update();
            
            if (StateTime < 0)
            {
                StateMachine.ChangeState(enemy.MoveState);
            }
        }
    }
}
