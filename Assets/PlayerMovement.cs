using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionReference moveInputAction;
    public InputActionReference jumpInputAction;
    public InputActionReference nextActivePartsAction;
    public InputActionReference previousActivePartsAction;

    public bool isAlive = true;
    public float moveSpeed = 20f;
    public float maxVelocity = 3f;
    public float jumpStrength = 20f;

    private Vector2 moveVector;
    private bool isGrounded;

    Rigidbody2D rb;

    private void OnEnable()
    {
        moveInputAction.action.performed += HandleMoveAction;
        moveInputAction.action.canceled += HandleStopMoveAction;

        jumpInputAction.action.performed += HandleJumpAction;

        nextActivePartsAction.action.performed += HandleNextActivePartsAction;
        previousActivePartsAction.action.performed += HandlePreviousActivePartsAction;
    }

    private void OnDisable()
    {
        moveInputAction.action.performed -= HandleMoveAction;
        moveInputAction.action.canceled -= HandleStopMoveAction;

        jumpInputAction.action.performed -= HandleJumpAction;

        nextActivePartsAction.action.performed -= HandleNextActivePartsAction;
        previousActivePartsAction.action.performed -= HandlePreviousActivePartsAction;
    }

    private void HandleMoveAction(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
        // horizontal movement only
        moveVector.y = 0;
    }

    private void HandleStopMoveAction(InputAction.CallbackContext context)
    {
        moveVector = Vector2.zero;
    }

    private void HandleJumpAction(InputAction.CallbackContext obj)
    {
        JumpOnInput();
    }

    private void HandleNextActivePartsAction(InputAction.CallbackContext obj)
    {
        Debug.Log("Next active part!");
        // TODO
    }

    private void HandlePreviousActivePartsAction(InputAction.CallbackContext obj)
    {
        Debug.Log("Previous active part!");
        // TODO
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        MoveOnInput();
    }

    private void MoveOnInput()
    {
        if (isAlive)
        {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            // TODO FUCK MAC
            moveVector.y = -moveVector.y;
#endif

            if (moveVector != Vector2.zero)
            {
                rb.AddForce(moveVector * moveSpeed);

                var velocity = rb.linearVelocity;
                velocity.x = Mathf.Clamp(velocity.x, -maxVelocity, maxVelocity);
                rb.linearVelocity = velocity;
            }
        }
    }

    private void JumpOnInput()
    {
        if (isAlive && isGrounded)
        {
            isGrounded = false;
            rb.AddForce(Vector2.up * jumpStrength * moveSpeed);
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxVelocity);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("SlideWall"))
        {
            isGrounded = true;
        }
    }
}