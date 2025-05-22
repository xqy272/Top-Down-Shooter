using Enemy.Enemy_Melee;
using UnityEngine;

namespace Enemy
{
    public class EnemyAnimationEvents : MonoBehaviour
    {
        private Enemy enemy;

        private void Awake()
        {
            enemy = GetComponentInParent<Enemy>();
        }
        
        public void AnimationTrigger() => enemy.AnimationTrigger();
        
        public void StartManualMovement() => enemy.ActivateMoveManually(true);
        public void StopManualMovement() => enemy.ActivateMoveManually(false);
        
        public void StartManualRotation() => enemy.ActivateRotationManually(true);
        public void StopManualRotation() => enemy.ActivateRotationManually(false);

        public void AbilityEvent() => enemy.GetComponent<EnemyMelee>().AbilityTrigger();
    }
}
