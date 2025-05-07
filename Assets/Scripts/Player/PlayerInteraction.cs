using System.Collections.Generic;
using Interactive_System;
using UnityEngine;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        private readonly List<Interactable> _interactables = new();
        private Interactable _closestInteractable;

        private void Start()
        {
            Player player = GetComponent<Player>();
            
            player.PlayerController.Character.Interaction.performed += _ => InteractWithClosestInteractable();
        }

        private void InteractWithClosestInteractable()
        {
            _closestInteractable?.Interaction();
            _interactables.Remove(_closestInteractable);
            
            UpdateClosestInteractable();
        }

        public void UpdateClosestInteractable()
        {
            _closestInteractable?.HighlightActive(false);
            
            _closestInteractable = null;
            float closestDistance = float.MaxValue;

            foreach(Interactable interactable in _interactables)
            {
                float distance = Vector3.Distance(transform.position, interactable.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _closestInteractable = interactable;
                }
            }
            _closestInteractable?.HighlightActive(true);
        }

        public List<Interactable> GetInteractables() => _interactables;
    }
}
