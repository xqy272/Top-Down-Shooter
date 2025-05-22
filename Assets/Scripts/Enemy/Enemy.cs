using System.Collections;
using Enemy.StateMachine;
using UnityEngine;
using UnityEngine.AI;



namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] protected int health = 20;
        
        [Header("Idle Data")]
        public float idleTime;
        public float aggressionRange;
        
        [Header("Move Data")]
        public float walkSpeed;
        public float chaseSpeed;
        public float turnSpeed;
        private bool _moveManually;
        private bool _rotationManually;
        
        [SerializeField] private Transform[] patrolPoints;
        private Vector3[] patrolPointsPositions;
        private int currentPatrolPointIndex;
        
        public Transform Player { get; private set; }
        public Animator Animator { get; private set; }
        public NavMeshAgent Agent { get; private set; }
        protected EnemyStateMachine StateMachine { get; private set; }
        protected bool InBattleMode { get; private set; }

        protected virtual void Awake()
        {
            StateMachine = new EnemyStateMachine();

            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponentInChildren<Animator>();
            Player = GameObject.Find("Player").GetComponent<Transform>();
        }
        
        protected virtual void Start()
        {
            InitializePatrolPoints();
        }

        protected virtual void Update()
        {
        }

        protected bool ShouldEnterBattleMode()
        {
            bool inAggressionRange = Vector3.Distance(transform.position, Player.position) < aggressionRange;

            if (!inAggressionRange || InBattleMode) return false;
            
            EnterBattleMode();
            return true;
        }

        protected virtual void EnterBattleMode()
        {
            InBattleMode = true;
        }
        
        public virtual void AbilityTrigger()
        {
            StateMachine.CurrentState.AbilityTrigger();
        }

        public virtual void GetHit()
        {
            health--;
            if(InBattleMode)
                return;
            EnterBattleMode();
        }

        public virtual void HitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
        {
            StartCoroutine(HitImpactCoroutine(force, hitPoint, rb));
        }

        private IEnumerator HitImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
        {
            yield return new WaitForSeconds(0.1f);
            
            rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
        }

        public void FaceTarget(Vector3 target)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
            
            Vector3 currentRotation = transform.rotation.eulerAngles;
            
            float yRotation = Mathf.LerpAngle(currentRotation.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);
            
            transform.rotation = Quaternion.Euler(currentRotation.x, yRotation, currentRotation.z);
        }
        
        #region Patrol logic
        public Vector3 GetPatrolDestination()
        {
            Vector3 patrolDestination = patrolPointsPositions[currentPatrolPointIndex];

            currentPatrolPointIndex++;
            
            if(currentPatrolPointIndex >= patrolPoints.Length)
                currentPatrolPointIndex = 0;
            return patrolDestination;
        }
        
        private void InitializePatrolPoints()
        {
            patrolPointsPositions = new Vector3[patrolPoints.Length];
            for (var i = 0; i < patrolPoints.Length; i++)
            {
                patrolPointsPositions[i] = patrolPoints[i].position;
                patrolPoints[i].gameObject.SetActive(false);
            }
        }
        #endregion
        
        #region Animation events
        public void ActivateMoveManually(bool activated) => _moveManually = activated;
        public bool IsMoveManually() => _moveManually;
        public void ActivateRotationManually(bool activated) => _rotationManually = activated;
        public bool IsRotationManually() => _rotationManually;
        public void AnimationTrigger() => StateMachine.CurrentState.AnimationTriggerCalled();
        #endregion
    }
}
