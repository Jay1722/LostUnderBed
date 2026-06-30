using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.3f); 
    public LayerMask groundLayer;

    [Header("Crouch Settings")]
    public SpriteRenderer spriteRenderer;
    private CapsuleCollider2D playerCollider;

    [Header("Ceiling Check Settings")]
    public Transform ceilingCheck; 
    public float ceilingCheckRadius = 0.2f;
    public LayerMask obstacleLayer; 

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset; 
    private Vector3 originalSpriteScale;    
    private bool isCrouching = false;
    private bool isCrouchButtonPressed = false; 

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGround;
    private Animator anim; // Variabel Animator

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>(); // Menghubungkan Animator

        if (playerCollider != null)
        {
            originalColliderSize = playerCollider.size;
            originalColliderOffset = playerCollider.offset; 
        }

        if (spriteRenderer != null)
        {
            originalSpriteScale = spriteRenderer.transform.localScale; 
        }
    }

    void FixedUpdate()
    {
        // 1. Cek Ground
        if (groundCheck != null)
        {
            isGround = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

            Color debugColor = isGround ? Color.green : Color.red;
            Vector2 halfSize = groundCheckSize / 2f;
            Debug.DrawLine(new Vector2(groundCheck.position.x - halfSize.x, groundCheck.position.y + halfSize.y), new Vector2(groundCheck.position.x + halfSize.x, groundCheck.position.y + halfSize.y), debugColor);
            Debug.DrawLine(new Vector2(groundCheck.position.x + halfSize.x, groundCheck.position.y + halfSize.y), new Vector2(groundCheck.position.x + halfSize.x, groundCheck.position.y - halfSize.y), debugColor);
            Debug.DrawLine(new Vector2(groundCheck.position.x + halfSize.x, groundCheck.position.y - halfSize.y), new Vector2(groundCheck.position.x - halfSize.x, groundCheck.position.y - halfSize.y), debugColor);
            Debug.DrawLine(new Vector2(groundCheck.position.x - halfSize.x, groundCheck.position.y - halfSize.y), new Vector2(groundCheck.position.x - halfSize.x, groundCheck.position.y + halfSize.y), debugColor);
        }

        // 2. Cek status Crouch dan Ceiling
        HandleCrouchState();

        // 3. Movement
        float currentSpeed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        rb.linearVelocity = new Vector2(moveInput.x * currentSpeed, rb.linearVelocity.y);

        // 4. Membalikkan Badan (Flip)
        if (moveInput.x > 0.01f) spriteRenderer.flipX = false;
        else if (moveInput.x < -0.01f) spriteRenderer.flipX = true;

        // 5. Update Semua Animasi
        if (anim != null)
        {
            bool isMoving = Mathf.Abs(moveInput.x) > 0.01f;
            anim.SetBool("isWalking", isMoving);
            
            // Logika baru: Jika isGround false (di udara), isJumping jadi true!
            anim.SetBool("isJumping", !isGround); 

            anim.SetFloat("velocityY", rb.linearVelocity.y);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGround && !isCrouching)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isCrouchButtonPressed = true;
        }
        else if (context.canceled)
        {
            isCrouchButtonPressed = false;
        }
    }

    private void HandleCrouchState()
    {
        if (playerCollider == null || spriteRenderer == null) return;

        bool canStand = true;

        if (ceilingCheck != null)
        {
            canStand = !Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, obstacleLayer);
        }

        if (isCrouchButtonPressed && !isCrouching)
        {
            SetCrouch(true);
        }
        else if (!isCrouchButtonPressed && isCrouching && canStand)
        {
            SetCrouch(false);
        }
    }

    private void SetCrouch(bool state)
    {
        isCrouching = state;

        // Tambahkan baris ini untuk mengaktifkan animasi di Animator
        if (anim != null)
        {
            anim.SetBool("isCrouching", isCrouching);
        }

        if (isCrouching)
        {
            // ... (Logika perubahan ukuran collider tetap sama seperti sebelumnya) ...
            float targetHeight = originalColliderSize.y * 0.5f;
            float heightDifference = originalColliderSize.y - targetHeight;
            playerCollider.size = new Vector2(originalColliderSize.x, targetHeight);
            playerCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (heightDifference / 2f));
            spriteRenderer.transform.localScale = new Vector3(originalSpriteScale.x, originalSpriteScale.y * 0.5f, originalSpriteScale.z);
        }
        else
        {
            // ... (Logika mengembalikan ukuran semula) ...
            playerCollider.size = originalColliderSize;
            playerCollider.offset = originalColliderOffset;
            spriteRenderer.transform.localScale = originalSpriteScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (ceilingCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ceilingCheck.position, ceilingCheckRadius);
        }
    }
}