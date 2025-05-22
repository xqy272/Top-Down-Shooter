using System.Collections.Generic;
using Enemy.StateMachine;
using UnityEngine;

namespace Enemy.Enemy_Melee
{
    public class AttackStateMelee : EnemyState
    {
        private static readonly int AttackAnimationSpeed = Animator.StringToHash("AttackAnimationSpeed");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
        private static readonly int RecoveryIndex = Animator.StringToHash("RecoveryIndex");
        private static readonly int SlashAttackIndex = Animator.StringToHash("SlashAttackIndex");
        private readonly EnemyMelee enemy;
        private Vector3 attackDirection;

        private const float MaxAttackDistance = 50f;
        private float attackMoveSpeed;
        
        public AttackStateMelee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
        {
            enemy = enemyBase as EnemyMelee;
        }

        public override void Enter()
        {
            base.Enter();
            
            enemy.PullWeapon();
            
            attackMoveSpeed = enemy.attackData.moveSpeed;
            enemy.Animator.SetFloat(AttackAnimationSpeed, enemy.attackData.animationSpeed);
            enemy.Animator.SetFloat(AttackIndex, enemy.attackData.attackIndex);
            enemy.Animator.SetFloat(SlashAttackIndex, Random.Range(0, 5));
            
            enemy.Agent.isStopped = true;
            enemy.Agent.velocity = Vector3.zero;
            
            attackDirection = enemy.transform.position + (enemy.transform.forward * MaxAttackDistance);
        }

        public override void Update()
        {
            base.Update();

            if (enemy.IsRotationManually())
            {
                enemy.FaceTarget(enemy.Player.position);
                attackDirection = enemy.transform.position + (enemy.transform.forward * MaxAttackDistance);
            }
                

            if (enemy.IsMoveManually())
            {
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackDirection, attackMoveSpeed * Time.deltaTime);
            }
            
            if (TriggerCalled)
            {
                if(enemy.PlayerInAttackRange())
                    StateMachine.ChangeState(enemy.RecoveryState);
                else
                    StateMachine.ChangeState(enemy.ChaseState);
            }
        }

        public override void Exit()
        {
            base.Exit();

            SetupNextAttack();
        }

        private void SetupNextAttack()
        {
            int recoveryIndex = PlayerClose() ? 1 : 0;
            
            enemy.Animator.SetFloat(RecoveryIndex,recoveryIndex);
            enemy.attackData = UpdatedAttackData();
        }

        private bool PlayerClose() => Vector3.Distance(enemy.transform.position, enemy.Player.position) <= 1;

        private AttackData UpdatedAttackData()
        {
            List<AttackData> validAttacks = new List<AttackData>(enemy.attackList);

            if (PlayerClose()) 
                validAttacks.RemoveAll(parameter => parameter.attackType == AttackTypeMelee.Charge);
            
            int random = Random.Range(0, validAttacks.Count);
            return validAttacks[random];
        }
    }
}
