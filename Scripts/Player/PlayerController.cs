using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float runSpeed = 2f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    Vector2 movementInput;
    Rigidbody2D rb;
    Animator animator;
    PlayerCustomization playerCustomization;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    PlayerInputActions playerInputActions;
    private bool isNetworked = false;
    private bool isSitting = false;
    private bool isRunning = false;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        isNetworked = GetComponent<NetworkIdentity>() != null;
    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable();
        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMove;
        playerInputActions.Player.Sit.performed += OnSit;
        playerInputActions.Player.Run.performed += OnRun;
        playerInputActions.Player.Run.canceled += OnRun;
    }

    private void OnDisable()
    {
        playerInputActions.Player.Move.performed -= OnMove;
        playerInputActions.Player.Move.canceled -= OnMove;
        playerInputActions.Player.Sit.performed -= OnSit;
        playerInputActions.Player.Run.performed -= OnRun;
        playerInputActions.Player.Run.canceled -= OnRun;
        playerInputActions.Player.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCustomization = GetComponent<PlayerCustomization>();

        // Definir o estado inicial
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveY", -1); // IdleDown por padrão
    }

    private void FixedUpdate()
    {
        if (isNetworked && !isLocalPlayer())
        {
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        if (movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput, currentSpeed);

            if (!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0), currentSpeed);

                if (!success)
                {
                    success = TryMove(new Vector2(0, movementInput.y), currentSpeed);
                }
            }

            UpdateAnimator(isRunning, true);
            UpdateDirection(movementInput);
        }
        else
        {
            UpdateAnimator(false, false);
        }
    }

    private bool TryMove(Vector2 direction, float speed)
    {
        int count = rb.Cast(
            direction,
            movementFilter,
            castCollisions,
            speed * Time.fixedDeltaTime + collisionOffset
        );

        if (count == 0) // Move if no collisions detected
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateAnimator(bool isRunning, bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning);
        playerCustomization.SetMovementState(isRunning, isMoving);
    }

    private void UpdateDirection(Vector2 movementInput)
    {
        if (Mathf.Abs(movementInput.x) > Mathf.Abs(movementInput.y))
        {
            if (movementInput.x > 0)
            {
                SetDirection(1, 0);
            }
            else
            {
                SetDirection(-1, 0);
            }
        }
        else
        {
            if (movementInput.y > 0)
            {
                SetDirection(0, 1);
            }
            else
            {
                SetDirection(0, -1);
            }
        }
    }

    private void SetDirection(float moveX, float moveY)
    {
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);
        playerCustomization.SetDirection(moveX, moveY);
    }

    void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        if (movementInput != Vector2.zero)
        {
            // Cancel sitting state when moving
            isSitting = false;
            animator.SetBool("isSitting", false);
            playerCustomization.SetSittingState(false);
        }
    }

    void OnSit(InputAction.CallbackContext context)
    {
        isSitting = !isSitting;
        animator.SetBool("isSitting", isSitting);
        playerCustomization.SetSittingState(isSitting);

        animator.Update(0f);
    }

    void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    private bool isLocalPlayer()
    {
        return !isNetworked || (isNetworked && GetComponent<NetworkIdentity>().isLocalPlayer);
    }
}
