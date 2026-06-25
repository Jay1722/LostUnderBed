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

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset; // KUNCI UTAMA: Wajib dicatat kembali!
    private Vector3 originalSpriteScale;    // KUNCI UTAMA: Mencatat skala asli milik Sprite Renderer, bukan parent
    private bool isCrouching = false;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGround;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();

        if (playerCollider != null)
        {
            originalColliderSize = playerCollider.size;
            originalColliderOffset = playerCollider.offset; // Catat offset awal
        }

        if (spriteRenderer != null)
        {
            originalSpriteScale = spriteRenderer.transform.localScale; // Catat skala asli sprite
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;

        rb.linearVelocity = new Vector2(moveInput.x * currentSpeed, rb.linearVelocity.y);

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

        Debug.Log("Ground: " + isGround + " | Crouch: " + isCrouching);
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
        if (playerCollider == null || spriteRenderer == null) return;

        if (context.started)
        {
            Debug.Log("Jongkok");
            isCrouching = true;

            float targetHeight = originalColliderSize.y * 0.5f;
            float heightDifference = originalColliderSize.y - targetHeight;

            // 1. Ubah ukuran fisik kapsul & geser offset Y-nya ke bawah agar kaki mengunci lantai
            playerCollider.size = new Vector2(originalColliderSize.x, targetHeight);
            playerCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (heightDifference / 2f));

            // 2. Ubah visual sprite mengecil ke bawah
            spriteRenderer.transform.localScale = new Vector3(
                originalSpriteScale.x,
                originalSpriteScale.y * 0.5f,
                originalSpriteScale.z
            );
        }
        else if (context.canceled)
        {
            Debug.Log("Berdiri");
            isCrouching = false;

            // Kembalikan ukuran fisik ke semula
            playerCollider.size = originalColliderSize;
            playerCollider.offset = originalColliderOffset;

            // Kembalikan visual ke ukuran semula yang akurat
            spriteRenderer.transform.localScale = originalSpriteScale;
        }
    }
}