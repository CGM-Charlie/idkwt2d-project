using System;
using UnityEngine;

namespace Kizuna.Player {
    public class Movement : MonoBehaviour {
        [Header("Components")] 
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private Animator animator;

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
        [SerializeField] private int jumps;
        [SerializeField] private float coyoteJumpTime;
        [SerializeField] private float jumpBufferTime;
        private int jumpCounter;
        private float coyoteJumpTimeCounter;
        private float jumpBufferCounter; // TODO: FIX JUMP BUFFER
        private bool isJumping;

        [Header("Ground Collision Variables")] 
        [SerializeField] private float groundRaycastLength;
        [SerializeField] private Vector3 groundRaycastOffset;
        private bool onGround;

        // Private Values
        private Vector2 inputVector;
        private float horizontalDirection;
        
        // Animator Values
        private bool isFacingRight = true;

        private bool ChangingDirection => 
            (rb2d.velocity.x > 0f && horizontalDirection < 0f) || (rb2d.velocity.x < 0f && horizontalDirection > 0f);
        
        private void Start() {
            // Assign Components
            rb2d = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            
            inputVector = Vector2.zero;
            horizontalDirection = inputVector.x;
        }
        
        private void Update() {
            horizontalDirection = inputVector.x;

            // Animation
            animator.SetBool("IsGrounded", onGround);
            animator.SetFloat("HorizontalDirection", Mathf.Abs(horizontalDirection));

            if (horizontalDirection < 0f && isFacingRight) {
                FlipCharacter();
            } else if (horizontalDirection > 0f && !isFacingRight) {
                FlipCharacter();
            }

            if (rb2d.velocity.y < 0f) {
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", true);
            }

        }

        private void FixedUpdate() {
            MoveCharacter();
            IsCharacterOnGround();

            // Rigid Body Drag
            if (onGround) {
                jumpCounter = jumps;
                coyoteJumpTimeCounter = coyoteJumpTime;
                ApplyGroundLinearDrag();
                
                // Animation
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", false);
            } else {
                coyoteJumpTimeCounter -= Time.deltaTime;
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
            if (Mathf.Abs(horizontalDirection) < 0.4f || ChangingDirection) {
                rb2d.drag = groundLinearDrag;
            } else {
                rb2d.drag = 0;
            }
        }

        private void ApplyAirLinearDrag() {
            rb2d.drag = airLinealDrag;
        }

        private void IsCharacterOnGround() {
            var position = transform.position;
            onGround = 
                Physics2D.Raycast(position + groundRaycastOffset, Vector2.down, groundRaycastLength, groundLayers) ||                 
                Physics2D.Raycast(position - groundRaycastOffset, Vector2.down, groundRaycastLength, groundLayers);
        }

        private void FlipCharacter() {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }

        // Gizmos
        private void OnDrawGizmos() {
            var position = transform.position;
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(position + groundRaycastOffset, position + groundRaycastOffset + Vector3.down * groundRaycastLength);
            Gizmos.DrawLine(position - groundRaycastOffset, position - groundRaycastOffset + Vector3.down * groundRaycastLength);

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
            if (coyoteJumpTimeCounter < 0f) {
                jumpCounter--;
            }

            if (coyoteJumpTimeCounter > 0f || jumpCounter > 0) {
                coyoteJumpTimeCounter = 0f;
                jumpBufferCounter = 0f;
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
                rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            
            // Animation
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);
        }
    }
}
