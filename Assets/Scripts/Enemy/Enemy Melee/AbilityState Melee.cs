using Enemy.StateMachine;
using Object_Pool;
using UnityEngine;

namespace Enemy.Enemy_Melee
{
    public class AbilityStateMelee : EnemyState
    {
        private static readonly int RecoveryIndex = Animator.StringToHash("RecoveryIndex");
        
        private readonly EnemyMelee enemy;
        
        private Vector3 movementDirection;
        
        private float moveSpeed;
        private float lastTimeAxeThrow;
        
        private const float MaxMovementDistance = 20;
        public AbilityStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
        {
            enemy = enemyBase as EnemyMelee;
        }

        public override void Enter()
        {
            base.Enter();
            
            enemy.PullWeapon();

            moveSpeed = enemy.walkSpeed;
            movementDirection = enemy.transform.position + (enemy.transform.forward * MaxMovementDistance);
        }

        public override void Update()
        {
            base.Update();
            
            if (enemy.IsRotationManually())
            {
                enemy.FaceTarget(enemy.Player.position);
                movementDirection = enemy.transform.position + (enemy.transform.forward * MaxMovementDistance);
            }
            
            if (enemy.IsMoveManually())
            {
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, movementDirection, moveSpeed * Time.deltaTime);
            }
            
            if(TriggerCalled)
                StateMachine.ChangeState(enemy.RecoveryState);
        }

        public override void Exit()
        {
            base.Exit();

            enemy.walkSpeed = moveSpeed;
            enemy.Animator.SetFloat(RecoveryIndex, 0);
        }

        public override void AbilityTrigger()
        {
            base.AbilityTrigger();
            
            GameObject newAxe = ObjectPool.Instance.GetObject(enemy.axePrefab);
            
            newAxe.transform.position = enemy.axeStartPoint.position;
            newAxe.GetComponent<EnemyAxe>().AxeSetup(enemy.Player, enemy.axeFlySpeed, enemy.axeRotationSpeed, enemy.attackTimer);
        }
    }
}
