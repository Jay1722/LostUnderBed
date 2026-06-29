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
    public Transform ceilingCheck; // Titik di atas kepala karakter saat jongkok
    public float ceilingCheckRadius = 0.2f;
    public LayerMask obstacleLayer; // Layer untuk rintangan/atap

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset; 
    private Vector3 originalSpriteScale;    
    private bool isCrouching = false;
    private bool isCrouchButtonPressed = false; // Menyimpan status input tombol

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
        // Hanya mencatat apakah tombol sedang ditekan atau dilepas
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

        // Mengecek apakah ada objek di atas karakter menggunakan OverlapCircle
        if (ceilingCheck != null)
        {
            canStand = !Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, obstacleLayer);
        }

        // Jika tombol ditekan dan belum jongkok -> Mulai jongkok
        if (isCrouchButtonPressed && !isCrouching)
        {
            SetCrouch(true);
        }
        // Jika tombol DILEPAS, sedang jongkok, DAN area atas kosong -> Berdiri
        else if (!isCrouchButtonPressed && isCrouching && canStand)
        {
            SetCrouch(false);
        }
        // Jika tombol dilepas, tapi area atas ADA rintangan, karakter akan tetap dalam state jongkok (isCrouching = true)
    }

    // Memisahkan logika visual & collider ke fungsi tersendiri
    private void SetCrouch(bool state)
    {
        isCrouching = state;

        if (isCrouching)
        {
            Debug.Log("Jongkok");
            float targetHeight = originalColliderSize.y * 0.5f;
            float heightDifference = originalColliderSize.y - targetHeight;

            playerCollider.size = new Vector2(originalColliderSize.x, targetHeight);
            playerCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (heightDifference / 2f));

            spriteRenderer.transform.localScale = new Vector3(
                originalSpriteScale.x,
                originalSpriteScale.y * 0.5f,
                originalSpriteScale.z
            );
        }
        else
        {
            Debug.Log("Berdiri");
            playerCollider.size = originalColliderSize;
            playerCollider.offset = originalColliderOffset;
            spriteRenderer.transform.localScale = originalSpriteScale;
        }
    }

    // Menampilkan lingkaran visual di editor Unity untuk memudahkan pengaturan Ceiling Check
    private void OnDrawGizmosSelected()
    {
        if (ceilingCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ceilingCheck.position, ceilingCheckRadius);
        }
    }
}