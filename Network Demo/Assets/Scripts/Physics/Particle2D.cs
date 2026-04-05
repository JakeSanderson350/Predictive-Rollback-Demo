using UnityEngine;

public class Particle2D : MonoBehaviour
{
    public const int FLOAT_CONVERSION = 1000;

    public Vector2Int velocity;
    public float damping;
    public Vector2Int acceleration;
    public Vector2 gravity = new Vector2(0, -9.8f);
    public float inverseMass;
    public Vector2Int accumulatedForces { get; private set; }

    /*public void FixedUpdate()
    {
        DoFixedUpdate(Time.fixedDeltaTime);
    }*/

    public void Tick(float dt)
    {
        DoFixedUpdate(dt);
    }
    
    public void DoFixedUpdate(float dt)
    {
        // Apply force from each attached ForceGenerator component
        System.Array.ForEach(GetComponents<ForceGenerator>(), generator => { if (generator.enabled) generator.UpdateForce(this); });

        Integrator.Integrate(this, (int)(dt * FLOAT_CONVERSION));
        ClearForces();
    }

    public void ClearForces()
    {
        accumulatedForces = Vector2Int.zero;
    }

    public void AddForce(Vector2 force)
    {
        accumulatedForces += new Vector2Int((int)force.x * FLOAT_CONVERSION, (int)force.y * FLOAT_CONVERSION);
    }

    public Particle2D GetUpdateParticle(float dt)
    {
        System.Array.ForEach(GetComponents<ForceGenerator>(), generator => { if (generator.enabled) generator.UpdateForce(this); });

        var temp = Integrator.TempIntegrate(this, dt);
        ClearForces();

        return temp;
    }
}
