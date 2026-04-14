using UnityEngine;
using UnityEngine.Rendering;

public class Particle2D : MonoBehaviour
{
    // Position
    public long positionX;
    public long positionY;

    // Velocity
    public long velocityX;
    public long velocityY;

    // Acceleration
    public long accelerationX;
    public long accelerationY;

    // Accumulated forces
    public long accumulatedForceX;
    public long accumulatedForceY;

    // 1/mass
    public long inverseMass;

    // Damping * FP_SCALE
    public long damping;

    /*public void FixedUpdate()
    {
        DoFixedUpdate(Time.fixedDeltaTime);
    }*/

    public void Tick(float dt)
    {
        DoFixedUpdate(dt);
    }
    
    public void TickWithTransform(float dt)
    {
        DoFixedUpdate(dt);
        ApplyStateToTransform();
    }
    
    public void DoFixedUpdate(float dt)
    {
        // Apply force from each attached ForceGenerator component
        System.Array.ForEach(GetComponents<ForceGenerator>(), generator => { if (generator.enabled) generator.UpdateForce(this); });

        Integrator.Integrate(this, PhysicsConstants.FP_DT);
        
        ClearForces();

        //ApplyStateToTransform();
    }

    public void ClearForces()
    {
        accumulatedForceX = 0;
        accumulatedForceY = 0;
    }

    public void AddForce(long fx, long fy)
    {
        accumulatedForceX += fx;
        accumulatedForceY += fy;
    }

    private void ApplyStateToTransform()
    {
        float wx = positionX / (float)PhysicsConstants.FP_SCALE;
        float wy = positionY / (float)PhysicsConstants.FP_SCALE;
        transform.position = new Vector3(wx, wy, transform.position.z);
    }

    public Particle2D GetUpdateParticle(float dt)
    {
        System.Array.ForEach(GetComponents<ForceGenerator>(), generator => { if (generator.enabled) generator.UpdateForce(this); });

        var temp = Integrator.Integrate(this, (long)(dt * PhysicsConstants.FP_SCALE));
        ClearForces();

        return temp;
    }
}
