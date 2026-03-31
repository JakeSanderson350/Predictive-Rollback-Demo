using UnityEngine;

public abstract class ForceGenerator : MonoBehaviour
{
    public abstract void UpdateForce(Particle2D particle);
}

