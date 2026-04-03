using UnityEngine;

public class PlayerMovementPhysics : ForceGenerator
{
    [Header("Move Variables")]
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float maxSpeed = 2f;

    public bool moveActivated = true;

    public Particle2D particle;

    public Vector2 moveInput { get; private set; }
    public Vector2 moveForce { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        particle = GetComponent<Particle2D>();
    }

    public void initializeClientInput(InputSystem_Actions.PlayerActions actions)
    {
        //actions.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // moveInput = 
        //actions.Move.canceled += ctx => moveInput = ctx.ReadValue<Vector2>();
    }

    public void SetMoveInput(Vector3 input)
    {
        input.y = input.z;
        moveInput = input;
    }


    // A function like this should be used to set inputs on the server
    // All this script needs to process movement is for the moveInput variable to be set each frame/update
    public void initalizeServerInput()
    {

    }

    /*private void FixedUpdate()
    {
        if (moveActivated)
        {
            Move();

            SetRotation();
        }

    }*/

    public void Tick()
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
        if ((moveInputValue.x > 0 && particle.velocity.x < 0) || (moveInputValue.x < 0 && particle.velocity.x > 0))
        {
            particle.velocity.Set(moveInputValue.x * moveSpeed, particle.velocity.y);
        }
        if ((moveInputValue.y > 0 && particle.velocity.y < 0) || (moveInputValue.y < 0 && particle.velocity.y > 0))
        {
            particle.velocity.Set(particle.velocity.x, moveInputValue.y * moveSpeed);
        }
        // -------------------- PLAYER MOVEMENT ----------------------- \\

        // doing the actual movement
        moveForce = moveInputValue * moveSpeed;

        // -------------------- CLAMP SPEED ----------------------- \\

        // clamp to a max speed to keep the acceleration on the move without a big mess
        particle.velocity = Vector2.ClampMagnitude(particle.velocity, (speedClamp));
    }

    public override void UpdateForce(Particle2D particle)
    {
        particle.AddForce(moveForce);
    }

    void SetRotation()
    {
        float angle = Mathf.Atan2(particle.velocity.y, particle.velocity.x) * Mathf.Rad2Deg - 90f;
    }

    public void SetMoveActivated(bool value)
    {
        moveActivated = value;
        if (!value)
        {
            particle.velocity = Vector3.zero;
        }
    }
}
