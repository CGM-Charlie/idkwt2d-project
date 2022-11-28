using System;
using UnityEngine;

namespace Kizuna.Player {
    public class Movement : MonoBehaviour {
        [Header("Components")] 
        [SerializeField] private Rigidbody2D rb2d;

        [Header("Movement Variables")] 
        [SerializeField] private float movementAcceleration;
        [SerializeField] private float maxMovementSpeed;
        [SerializeField] private float linearDrag;

        [Header("Jump Variables")] 
        [SerializeField] private float jumpForce;
    
        // Private Values
        private Vector2 inputVector;
        private float horizontalDirection;

        private bool changingDirection => 
            (rb2d.velocity.x > 0f && horizontalDirection < 0f) || (rb2d.velocity.x < 0f && horizontalDirection > 0f);
        
        private void Start() {
            // Assign Components
            rb2d = GetComponent<Rigidbody2D>();
            
            inputVector = Vector2.zero;
            horizontalDirection = inputVector.x;
        }
        
        private void Update() {
            horizontalDirection = inputVector.x;
        }

        private void FixedUpdate() {
            MoveCharacter();
            ApplyLinearDrag();
        }
        
        private void MoveCharacter() {
            // Add force
            rb2d.AddForce(new Vector2(horizontalDirection, 0f) * movementAcceleration);
            
            // Clamp Speed
            if (Mathf.Abs(rb2d.velocity.x) > maxMovementSpeed) {
                rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxMovementSpeed, rb2d.velocity.y);
            }
        }

        // TODO: Fix Drag in vertical axis
        private void ApplyLinearDrag() {
            if (Mathf.Abs(horizontalDirection) < 0.4f || changingDirection) {
                rb2d.drag = linearDrag;
            } else {
                rb2d.drag = 0;
            }
        }
        
        // Public Methods
        
        public void SetInput(Vector2 vector2) {
            inputVector = vector2;
        }

        public void Jump() {
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
