using UnityEngine;

public static class Integrator
{
    public static Particle2D Integrate(Particle2D p, long dt)
    {
        // Update position:  pos += velocity * dt
        p.positionX += (p.velocityX * dt) / PhysicsConstants.FP_SCALE;
        p.positionY += (p.velocityY * dt) / PhysicsConstants.FP_SCALE;

        // Compute acceleration:  a = F * inverseMass + gravity
        p.accelerationX = (p.accumulatedForceX * p.inverseMass) / PhysicsConstants.FP_SCALE + PhysicsConstants.FP_GRAVITY_X;
        p.accelerationY = (p.accumulatedForceY * p.inverseMass) / PhysicsConstants.FP_SCALE + PhysicsConstants.FP_GRAVITY_Y;

        // Update velocity:  vel += acceleration * dt
        p.velocityX += (p.accelerationX * dt) / PhysicsConstants.FP_SCALE;
        p.velocityY += (p.accelerationY * dt) / PhysicsConstants.FP_SCALE;

        // Apply damping:  vel *= dampingFactor
        p.velocityX = (p.velocityX * p.damping) / PhysicsConstants.FP_SCALE;
        p.velocityY = (p.velocityY * p.damping) / PhysicsConstants.FP_SCALE;

        return p;
    }

    //public static Particle2D TempIntegrate(Particle2D particle, float dt)
    //{
    //    Particle2D temp = new Particle2D();
        
    //    //this transfrom position will not work
    //    temp.transform.position = particle.transform.position;
    //    temp.velocity = particle.velocity;
    //    temp.acceleration = particle.acceleration;
    //    temp.inverseMass = particle.inverseMass;
    //    temp.damping = particle.damping;
        
        
    //    temp.transform.position += (temp.velocity * dt).ToVector3(0);

    //    temp.acceleration = temp.accumulatedForces * temp.inverseMass + temp.gravity;

    //    temp.velocity += temp.acceleration * dt;
    //    temp.velocity *= Mathf.Pow(temp.damping, dt);
        
    //    return temp;
    //}
}

public static class VectorExtensions
{
    public static Vector3 New(Vector2 xy, float z)
    {
        return new Vector3(xy.x, xy.y, z);
    }

    public static Vector3 ToVector3(this Vector2 xy, float z)
    {
        return new Vector3(xy.x, xy.y, z);
    }
}
