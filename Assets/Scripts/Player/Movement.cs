using System;
using UnityEngine;

namespace Kizuna.Player {
    public class Movement : MonoBehaviour {
        [Header("Components")] 
        [SerializeField] private Rigidbody2D rb2d;

        [Header("Layer Masks")] 
        [SerializeField] private LayerMask groundLayers;

        // TODO: SET DEFAULT VALUES
        
        [Header("Movement Variables")] 
        [SerializeField] private float movementAcceleration;
        [SerializeField] private float maxMovementSpeed;
        [SerializeField] private float groundLinearDrag;

        [Header("Jump Variables")] 
        [SerializeField] private float jumpForce;
        [SerializeField] private float airLinealDrag;
        [SerializeField] private float fallMultiplayer;
        [SerializeField] private float lowJumpFallMultiplayer;
        [SerializeField] private int extraJumps;
        private int jumpCount;
        
        private bool isJumping;

        [Header("Ground Collision Variables")] 
        [SerializeField] private float groundRaycastLength;
        private bool onGround;

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
            IsCharacterOnGround();

            // Rigid Body Drag
            if (onGround) {
                jumpCount = extraJumps;
                ApplyGroundLinearDrag();
            } else {
                ApplyAirLinearDrag();
                FallMultiplier();
            }
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
        private void ApplyGroundLinearDrag() {
            if (Mathf.Abs(horizontalDirection) < 0.4f || changingDirection) {
                rb2d.drag = groundLinearDrag;
            } else {
                rb2d.drag = 0;
            }
        }

        private void ApplyAirLinearDrag() {
            rb2d.drag = airLinealDrag;
        }

        private void IsCharacterOnGround() {
            onGround = 
                Physics2D.Raycast(
                    transform.position * groundRaycastLength, 
                    Vector2.down, 
                    groundRaycastLength, 
                    groundLayers
                );
        }

        // Gizmos
        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastLength);
        }
        
        // Fall
        private void FallMultiplier() {
            if (rb2d.velocity.y < 0) {
                rb2d.gravityScale = fallMultiplayer;
            } else if (rb2d.velocity.y > 0 && !isJumping) {
                rb2d.gravityScale = lowJumpFallMultiplayer;
            } else {
                rb2d.gravityScale = 1f;
            }
        }

        // Public Methods
        
        public void SetInput(Vector2 vector2) {
            inputVector = vector2;
        }

        public void SetIsJumping(bool value) {
            isJumping = value;
        }

        // TODO: FIX JUMPS
        public void Jump() {
            if (!onGround) {
                jumpCount--;
            }
            
            if (onGround || jumpCount > 0) {
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
                rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }
}
