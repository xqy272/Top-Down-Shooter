using UnityEngine;
using UnityEngine.AI;

namespace Enemy.StateMachine
{
    public class EnemyState
    {
        protected readonly Enemy EnemyBase;
        protected readonly EnemyStateMachine StateMachine;

        private readonly string _animBoolName;
        protected float StateTime;

        protected bool TriggerCalled;
        
        protected EnemyState(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName)
        {
            EnemyBase = enemyBase;
            StateMachine = stateMachine;
            _animBoolName = animBoolName;
        }

        public virtual void Enter()
        {
            EnemyBase.Animator.SetBool(_animBoolName, true);
            
            TriggerCalled = false;
        }
        
        public virtual void Update()
        {
            StateTime -= Time.deltaTime;
        }

        public virtual void Exit()
        {
            EnemyBase.Animator.SetBool(_animBoolName, false);
        }

        protected Vector3 GetNextPathPoint()
        {
            NavMeshAgent agent = EnemyBase.Agent;
            NavMeshPath path = agent.path;

            if (path.corners.Length < 2)
                return agent.destination;
            for (int i = 0; i < path.corners.Length; i++)
            {
                if(Vector3.Distance(agent.transform.position, path.corners[i]) < 1)
                    return path.corners[i + 1];
            }
            return agent.destination;
        }
        
        public bool AnimationTriggerCalled() => TriggerCalled = true;
        
        public virtual void AbilityTrigger() { }
    }
}
