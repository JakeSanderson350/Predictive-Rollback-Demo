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
