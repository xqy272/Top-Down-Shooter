using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Enemy_Melee
{
    [System.Serializable]
    public struct AttackData
    {
        public string attackName;
        public float attackRange;
        public float moveSpeed;
        public int attackIndex;
        [Range(1, 3)] public float animationSpeed;
        public AttackTypeMelee attackType;
    }
    public enum AttackTypeMelee { Close, Charge }
    public enum EnemyMeleeType { Regular, Shield, Dodge, AxeThrow }
    
    public class EnemyMelee : Enemy
    {
        private static readonly int ChaseIndex = Animator.StringToHash("ChaseIndex");
        private static readonly int Dodge = Animator.StringToHash("Dodge");

        #region States
        public IdleStateMelee IdleState { get; private set; }
        public MoveStateMelee MoveState { get; private set; }
        public RecoveryStateMelee RecoveryState { get; private set; }
        public ChaseStateMelee ChaseState { get; private set; }
        public AttackStateMelee AttackState { get; private set; }
        private DeadStateMelee DeadState { get; set; }
        public AbilityStateMelee AbilityState { get; private set; }
        #endregion
        
        [Header("Enemy Setting")]
        public EnemyMeleeType enemyMeleeType;
        public Transform shieldTransform;
        public float dodgeCooldown;
        private float lastDodgeTime = -10;
        
        [Header("Axe Throw Ability")]
        public GameObject axePrefab;
        public float axeFlySpeed;
        public float axeRotationSpeed;
        public float attackTimer;
        public float axeThrowCooldown;
        private float lastAxeThrowTime;
        public Transform axeStartPoint;
        
        
        [Header("Attack Data")]
        public AttackData attackData;
        public List<AttackData> attackList;

        [SerializeField] private Transform hiddenWeapon;
        [SerializeField] private Transform pulledWeapon;

        protected override void Awake()
        {
            base.Awake();
            
            IdleState = new IdleStateMelee(this, StateMachine, "Idle");
            MoveState = new MoveStateMelee(this, StateMachine, "Move");
            RecoveryState = new RecoveryStateMelee(this, StateMachine, "Recovery");
            ChaseState = new ChaseStateMelee(this, StateMachine, "Chase");
            AttackState = new AttackStateMelee(this, StateMachine, "Attack");
            DeadState = new DeadStateMelee(this, StateMachine, "Dead");
            AbilityState = new AbilityStateMelee(this, StateMachine, "AxeThrow");
        }

        protected override void Start()
        {
            base.Start();
            StateMachine.Initialize(IdleState);
            
            InitializeSpeciality();
        }

        protected override void Update()
        {
            base.Update();
            StateMachine.CurrentState.Update();

            if (ShouldEnterBattleMode())
            {
                EnterBattleMode();
            }
        }

        protected override void EnterBattleMode()
        {
            if(InBattleMode)
                return;
            
            base.EnterBattleMode();
            
            StateMachine.ChangeState(RecoveryState);
        }

        private void InitializeSpeciality()
        {
            if (enemyMeleeType == EnemyMeleeType.Shield)
            {
                Animator.SetFloat(ChaseIndex, 1);
                shieldTransform.gameObject.SetActive(true);
            }
        }

        public override void AbilityTrigger()
        {
            base.AbilityTrigger();
            
            walkSpeed = walkSpeed * 0.6f;
            pulledWeapon.gameObject.SetActive(false);
        }

        public override void GetHit()
        {
            base.GetHit();
            
            if(health <= 0)
                StateMachine.ChangeState(DeadState);
        }

        public void PullWeapon()
        {
            hiddenWeapon.gameObject.SetActive(false);
            pulledWeapon.gameObject.SetActive(true);
        }

        public void ActivateDodgeRoll()
        {
            if(enemyMeleeType != EnemyMeleeType.Dodge)
                return;
            if(StateMachine.CurrentState != ChaseState)
                return;
            if(Vector3.Distance(transform.position, Player.position) < 2f)
                return;
            
            float animationClipDuration = GetAnimationClipDuration("Dodge roll");
            if (!(Time.time > animationClipDuration + lastDodgeTime + dodgeCooldown))
                return;
            
            lastDodgeTime = Time.time;
            Animator.SetTrigger(Dodge);
        }

        public bool CanThrowAxe()
        {
            if(enemyMeleeType != EnemyMeleeType.AxeThrow)
                return false;
            if (!(Time.time > lastAxeThrowTime + axeThrowCooldown))
                return false;
            
            lastAxeThrowTime = Time.time;
            return true;
        }

        private float GetAnimationClipDuration(string clipName)
        {
            AnimationClip[] clips = Animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                if (clip.name == clipName)
                    return clip.length;
            }
            return 0;
        }
        
        public bool PlayerInAttackRange() => Vector3.Distance(transform.position, Player.position) < attackData.attackRange;
         
        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawWireSphere(transform.position, aggressionRange);
        //     
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
        // }
    }
}
