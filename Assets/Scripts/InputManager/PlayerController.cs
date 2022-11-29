using System;
using Kizuna.Player;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: Investigate if it is possible to have different scripts for controlling the different input maps rather than using a single script
// TODO: Investigate if is possible to use the Master Singleton Structure for the input maps. Singleton InputManager that regulates all controllers

namespace Kizuna.InputManager {
    public class PlayerController : MonoBehaviour {
        // Auto-generated Input Maps
        public PlayerInputActions playerInputActions;
        
        // Input Components
        [Header("Player Components")] 
        [SerializeField] private Movement movement;

        private void Awake() {
            playerInputActions = new PlayerInputActions();
            SubscribePlayerMap(); // TODO: MOVE TO GAME MANAGER
        }

        private void Update() {
            Movement_MoveCharacter();
        }

        public void SubscribePlayerMap() {
            // Enable Map
            playerInputActions.PlayerController.Enable();
            
            // Subscribe Methods
            playerInputActions.PlayerController.Jump.performed += Movement_Jump;
            playerInputActions.PlayerController.Jump.canceled += Movement_Jump;
        }

        public void DisablePlayerMap() {
            // Unsubscribe Methods
            playerInputActions.PlayerController.Jump.performed -= Movement_Jump;
            playerInputActions.PlayerController.Jump.canceled -= Movement_Jump;
            
            // Disable Map
            playerInputActions.PlayerController.Disable();
        }

        // Player Movement
        private void Movement_MoveCharacter() {
            var inputVector = playerInputActions.PlayerController.Move.ReadValue<Vector2>();
            movement.SetInput(inputVector);
        }
        
        private void Movement_Jump(InputAction.CallbackContext context) {
            if (context.performed) {
                movement.Jump();
                movement.SetIsJumping(true);
            }

            if (context.canceled) {
                movement.SetIsJumping(false);
            }
        }
    }
}
