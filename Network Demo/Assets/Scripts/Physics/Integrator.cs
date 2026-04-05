using UnityEngine;

public static class Integrator
{
    public static void Integrate(Particle2D particle, int dt)
    {
        particle.transform.position += ((particle.velocity * dt) / (Particle2D.FLOAT_CONVERSION * 2)).ToVector3(0);

        particle.acceleration = particle.accumulatedForces * (int)(particle.inverseMass * Particle2D.FLOAT_CONVERSION) /*+ (particle.gravity * Particle2D.FLOAT_CONVERSION)*/;

        particle.velocity += particle.acceleration * dt;
        
        float damping = Mathf.Pow(particle.damping, dt / 1000.0f);
        Debug.Log(damping);
        particle.velocity = new Vector2Int((int)(particle.velocity.x * damping), (int)(particle.velocity.y * damping));
    }

    public static Particle2D TempIntegrate(Particle2D particle, float dt)
    {
        //Particle2D temp = new Particle2D();

        //this transfrom position will not work
        //temp.transform.position = particle.transform.position;
        //temp.velocity = particle.velocity;
        //temp.acceleration = particle.acceleration;
        //temp.inverseMass = particle.inverseMass;
        //temp.damping = particle.damping;


        //temp.transform.position += (temp.velocity * dt).ToVector3(0);

        //temp.acceleration = temp.accumulatedForces * temp.inverseMass + temp.gravity;

        //temp.velocity += temp.acceleration * dt;
        //temp.velocity *= Mathf.Pow(temp.damping, dt);

        //return temp;
        return null;
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

    public static Vector3 ToVector3(this Vector2Int xy, float z)
    {
        return new Vector3(xy.x, xy.y, z);
    }

    public static Vector2Int ClampMagnitude(Vector2Int vector, float minMagnitude)
    {
        Vector2 clamped = Vector2.ClampMagnitude(vector, minMagnitude);
        return new Vector2Int((int)clamped.x, (int)clamped.y);
    }
}
