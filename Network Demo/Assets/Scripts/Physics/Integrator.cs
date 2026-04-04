using UnityEngine;

public static class Integrator
{
    public static void Integrate(Particle2D particle, float dt)
    {
        particle.transform.position += (particle.velocity * dt).ToVector3(0);

        particle.acceleration = particle.accumulatedForces * particle.inverseMass + particle.gravity;

        particle.velocity += particle.acceleration * dt;
        particle.velocity *= Mathf.Pow(particle.damping, dt);
    }

    public static Particle2D TempIntegrate(Particle2D particle, float dt)
    {
        Particle2D temp = new Particle2D();
        
        //this transfrom position will not work
        temp.transform.position = particle.transform.position;
        temp.velocity = particle.velocity;
        temp.acceleration = particle.acceleration;
        temp.inverseMass = particle.inverseMass;
        temp.damping = particle.damping;
        
        
        temp.transform.position += (temp.velocity * dt).ToVector3(0);

        temp.acceleration = temp.accumulatedForces * temp.inverseMass + temp.gravity;

        temp.velocity += temp.acceleration * dt;
        temp.velocity *= Mathf.Pow(temp.damping, dt);
        
        return temp;
    }
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
