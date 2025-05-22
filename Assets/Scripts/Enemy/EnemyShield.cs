using Enemy.Enemy_Melee;
using UnityEngine;

namespace Enemy
{
    public class EnemyShield : MonoBehaviour
    {
        private static readonly int ChaseIndex = Animator.StringToHash("ChaseIndex");
        private EnemyMelee enemy;
        [SerializeField] public int durability;

        private void Awake()
        {
            enemy = GetComponentInParent<EnemyMelee>();
        }

        public void ReduceDurability()
        {
            durability--;

            if (durability <= 0)
            {
                enemy.Animator.SetFloat(ChaseIndex, 0);
                Destroy(gameObject);
            }
            
        }
    }
}
