using Player;
using UnityEngine;

namespace Interactive_System
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] private Material highlightMaterial;
        private Material _defaultMaterial;
        private MeshRenderer _meshRenderer;
        protected PlayerWeaponController PlayerWeaponController;

        private void Start()
        {
            if(_meshRenderer == null)
                _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _defaultMaterial = _meshRenderer.sharedMaterial;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (PlayerWeaponController == null)
                PlayerWeaponController = other.GetComponent<PlayerWeaponController>();
        
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

            if (playerInteraction == null) return;
        
            playerInteraction.GetInteractables().Add(this);
            playerInteraction.UpdateClosestInteractable();
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

            if (playerInteraction == null) return;
        
            playerInteraction.GetInteractables().Remove(this);
            playerInteraction.UpdateClosestInteractable();

        }

        protected void UpdateMeshAndMaterial(MeshRenderer newMeshRenderer)
        {
            _meshRenderer = newMeshRenderer; 
            _defaultMaterial = newMeshRenderer.sharedMaterial;
        }

        public void HighlightActive(bool active)
        {
            _meshRenderer.material = active ? highlightMaterial : _defaultMaterial;
        }

        public virtual void Interaction()
        {
        
        }
    }
}
