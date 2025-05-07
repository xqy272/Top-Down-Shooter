using UnityEngine;

namespace Weapon_System
{
    public enum HangType {LowBackHang, BackHang, SideHang}

    public class BackupWeaponModel : MonoBehaviour
    {
        public WeaponType weaponType;

        [SerializeField] private HangType hangType;

        public void Activate(bool activated) => gameObject.SetActive(activated);

        public bool IsHangTypeMatch(HangType targetHangType) => this.hangType == targetHangType;
    }
}