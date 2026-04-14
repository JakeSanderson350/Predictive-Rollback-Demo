using System.Numerics;
using UnityEngine;
using static Unity.Collections.Unicode;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovementPhysics : ForceGenerator, IPhySim<Particle2D>
{
    [SerializeField] bool isLocalPrediction = false;

    [Header("Move Variables")]
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float maxSpeed = 2f;

    public bool moveActivated = true;

    public Particle2D particle;

    public Vector2 moveInput { get; private set; }
    
    // Move force
    private long moveForceX;
    private long moveForceY;

    // Start is called before the first frame update
    void Awake()
    {
        particle = GetComponent<Particle2D>();
    }

    public void initializeClientInput(InputSystem_Actions.PlayerActions actions)
    {
        if (!isLocalPrediction)
            return;

        actions.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // moveInput = 
        actions.Move.canceled += ctx => moveInput = ctx.ReadValue<Vector2>();
    }

    public void SetMoveInput(Vector3 input)
    {
        if (isLocalPrediction)
            return;

        moveInput = input;
    }


    // A function like this should be used to set inputs on the server
    // All this script needs to process movement is for the moveInput variable to be set each frame/update
    public void initalizeServerInput()
    {

    }

    /*private void FixedUpdate()
    {
        if (isLocalPrediction)
        {
            Tick();
            particle.Tick(Time.deltaTime);
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
    void Move(Vector2? inPutDirection = null)
    {
        float speedClamp = maxSpeed;
        Vector2? temp = inPutDirection != null ? inPutDirection : moveInput;
        long inputX = (long)(((Vector2)temp).x * PhysicsConstants.FP_SCALE);
        long inputY = (long)(((Vector2)temp).y * PhysicsConstants.FP_SCALE);

        // -------------------- SMOOTH INVERSION ----------------------- \\
        // These if statements are making redirecting movement go faster
        // so if you are moving left, then swap to moving right, you'll switch velocities super fast
        // should be snappy but not jarring
        if ((inputX > 0 && particle.velocityX < 0) || (inputX < 0 && particle.velocityX > 0))
        {
            // velocity.x = inputX * moveSpeed   (both already in FP units/s)
            particle.velocityX = (long)(inputX * (moveSpeed * PhysicsConstants.FP_SCALE)) / PhysicsConstants.FP_SCALE;
        }
        if ((inputY > 0 && particle.velocityY < 0) || (inputY < 0 && particle.velocityY > 0))
        {
            particle.velocityY = (long)(inputY * (moveSpeed * PhysicsConstants.FP_SCALE)) / PhysicsConstants.FP_SCALE;
        }
        // -------------------- PLAYER MOVEMENT ----------------------- \\

        // doing the actual movement
        moveForceX = (long)(inputX * (moveSpeed * PhysicsConstants.FP_SCALE));
        moveForceY = (long)(inputY * (moveSpeed * PhysicsConstants.FP_SCALE));

        // -------------------- CLAMP SPEED ----------------------- \\

        // clamp to a max speed to keep the acceleration on the move without a big mess
        ClampVelocity((long)(maxSpeed * PhysicsConstants.FP_SCALE));
    }

    /// <summary>
    /// Clamps particle velocity to maxSpeed using integer squared-magnitude comparison.
    /// No sqrt, no float — fully deterministic.
    /// </summary>
    private void ClampVelocity(long maxSpeed)
    {
        // Squared magnitudes — use long to avoid overflow (values up to ~4e12 fit fine)
        long vx = particle.velocityX;
        long vy = particle.velocityY;

        // Scale down before squaring to prevent overflow:
        // (v / FP_SCALE)^2 compared to (maxSpeed / FP_SCALE)^2
        long vxS = vx / (PhysicsConstants.FP_SCALE / 1000); // scale to milli-units
        long vyS = vy / (PhysicsConstants.FP_SCALE / 1000);
        long maxS = maxSpeed / (PhysicsConstants.FP_SCALE / 1000);

        long sqMag = vxS * vxS + vyS * vyS;
        long sqMax = maxS * maxS;

        if (sqMag > sqMax && sqMag > 0)
        {
            // Integer approximation of sqrt via Newton's method (2–3 iterations).
            long mag = IntSqrt(sqMag);
            if (mag > 0)
            {
                // Rescale back: velocity = velocity * (maxSpeed / magnitude)
                // Work in milli-units to stay integer throughout.
                particle.velocityX = (vxS * maxS / mag) * (PhysicsConstants.FP_SCALE / 1000);
                particle.velocityY = (vyS * maxS / mag) * (PhysicsConstants.FP_SCALE / 1000);
            }
        }
    }

    /// <summary>
    /// Integer square root via Newton-Raphson. Deterministic on all platforms.
    /// Returns floor(sqrt(n)).
    /// </summary>
    private static long IntSqrt(long n)
    {
        if (n < 0) return 0;
        if (n == 0) return 0;
        long x = n;
        long y = (x + 1) / 2;
        while (y < x)
        {
            x = y;
            y = (x + n / x) / 2;
        }
        return x;
    }

    public override void UpdateForce(Particle2D particle)
    {
        particle.AddForce(moveForceX, moveForceY);
    }

    void SetRotation()
    {
        float angle = Mathf.Atan2(particle.velocityX / (float)PhysicsConstants.FP_SCALE
            , particle.velocityY / (float)PhysicsConstants.FP_SCALE) 
            * Mathf.Rad2Deg - 90f;
    }

    public void SetMoveActivated(bool value)
    {
        moveActivated = value;
        if (!value)
        {
            particle.velocityX = 0;
            particle.velocityY = 0;
        }
    }


    public Particle2D SimulateRaw(Vector3 inputDirection, Particle2D inputValue)
    {
        if (inputValue == null)
        {
            Move(inputDirection);
            particle.AddForce(moveForceX, moveForceY);
            return particle.GetUpdateParticle(Time.deltaTime);
        }
        else
        {
            Move(inputDirection);
            inputValue.AddForce(moveForceX, moveForceY);
            return inputValue.GetUpdateParticle(Time.deltaTime);
        }
      
    }

    public Vector3 Simulate()
    {
       //dw about this 
       return new Vector3();
    }
}
