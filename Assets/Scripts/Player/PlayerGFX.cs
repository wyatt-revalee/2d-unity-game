using System;
using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2d;
    public Animator animator;
    public Player player;
    public PlayerMovement playerMovement;

    // Movement
    private bool isCrouching;
    private bool isJumping;
    private bool isFalling;
    public int direction;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        direction = 1;

    }

    // Update is called once per frame
    void Update() {

        HandleMovementAnimations();

        animator.SetFloat("Speed", Math.Abs(rigidbody2d.velocity.x));
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetBool("IsGrounded", playerMovement.IsGrounded());
        animator.SetBool("IsFalling", isFalling);

        SetCharDirection();
        
    }

    private void HandleMovementAnimations() {

        // Jumping and falling
        if (!playerMovement.IsGrounded() && rigidbody2d.velocity.y >= 0) {
            isJumping = true;
        }
        else if(!playerMovement.IsGrounded() && rigidbody2d.velocity.y < 0) {
            isJumping = false;
            isFalling = true;
        } else {
            isJumping = false;
            isFalling = false;
        }

            // Crouch
        if (playerMovement.IsCrouching()) {
            isCrouching = true;
        } else {
            isCrouching = false;
        }
    }

    private void SetCharDirection() {

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            direction = -1;
        else if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            direction = 1;


        bool flipX = (direction < 0.0f)
        ? true
        : false;

        spriteRenderer.flipX = flipX;
    }
}
