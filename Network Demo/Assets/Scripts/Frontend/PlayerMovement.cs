using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move Variables")]
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float maxSpeed = 2f;

    float baseDrag;
    public bool moveActivated = true;

    Rigidbody2D rb;

    public Vector2 moveInput { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseDrag = rb.linearDamping;
    }

    public void initializeClientInput(InputSystem_Actions.PlayerActions actions)
    {
        actions.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // moveInput = 
        actions.Move.canceled += ctx => moveInput = ctx.ReadValue<Vector2>();
    }

    // A function like this should be used to set inputs on the server
    // All this script needs to process movement is for the moveInput variable to be set each frame/update
    public void initalizeServerInput()
    {
        
    }

    private void FixedUpdate()
    {
        if (moveActivated)
        {
            Move();

            SetRotation();
        }

    }
    void Move()
    {
        float speedClamp = maxSpeed;
        Vector2 moveInputValue = moveInput;

        // -------------------- SMOOTH INVERSION ----------------------- \\
        // These if statements are making redirecting movement go faster
        // so if you are moving left, then swap to moving right, you'll switch velocities super fast
        // should be snappy but not jarring
        if ((moveInputValue.x > 0 && rb.linearVelocityX < 0) || (moveInputValue.x < 0 && rb.linearVelocityX > 0))
        {
            rb.linearVelocity.Set(moveInputValue.x * moveSpeed, rb.linearVelocityY);
        }
        if ((moveInputValue.y > 0 && rb.linearVelocityY < 0) || (moveInputValue.y < 0 && rb.linearVelocityY > 0))
        {
            rb.linearVelocity.Set(rb.linearVelocityX, moveInputValue.y * moveSpeed);
        }
        // -------------------- PLAYER MOVEMENT ----------------------- \\

        // doing the actual movement
        rb.AddForce(moveInputValue * moveSpeed);

        // -------------------- CLAMP SPEED ----------------------- \\

        // clamp to a max speed to keep the acceleration on the move without a big mess
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, (speedClamp));
    }

    void SetRotation()
    {
        float angle = Mathf.Atan2(rb.linearVelocityY, rb.linearVelocityX) * Mathf.Rad2Deg - 90f;
    }

    public void SetMoveActivated(bool value)
    {
        moveActivated = value;
        if (!value)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}