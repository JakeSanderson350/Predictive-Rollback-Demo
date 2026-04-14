using UnityEngine;
using UnityEngine.UIElements;

public class LocalSimulationManager : MonoBehaviour
{
    [SerializeField] GameObject localSimulationPrefab;
    PlayerMovementPhysics localPm;

    public static LocalSimulationManager instance;

    private Player playerRef;

    private GameObject localSimulationRef;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        playerRef = GetComponent<Player>();

        if (playerRef.HasInputAuthority)
        {
            //localSimulationRef = Instantiate(localSimulationPrefab, transform.position, transform.rotation);
            localSimulationRef = Instantiate(localSimulationPrefab, transform.position, transform.rotation);
            localPm = localSimulationRef.GetComponent<PlayerMovementPhysics>();
            instance = this;
        }

        // Sync spawn point
        Particle2D particle = localPm.particle;
        particle.positionX = (long)(playerRef.transform.position.x * PhysicsConstants.FP_SCALE);
        particle.positionY = (long)(playerRef.transform.position.y * PhysicsConstants.FP_SCALE);
    }

    public void SetCorrection(Vector3 position)
    {
        localSimulationRef.transform.position = position;

        Particle2D particle = localPm.particle;
        particle.positionX = (long)(position.x * PhysicsConstants.FP_SCALE);
        particle.positionY = (long)(position.y * PhysicsConstants.FP_SCALE);
    }

    public void SyncLocalWithServer()
    {
        if (localSimulationRef != null)
        {
            localSimulationRef.transform.position = transform.position;

            Particle2D particle = localPm.particle;
            particle.positionX = (long)(transform.position.x * PhysicsConstants.FP_SCALE);
            particle.positionY = (long)(transform.position.y * PhysicsConstants.FP_SCALE);
        }
    }
    
    public void SyncLocalWithServerInput(Vector3 position)
    {
        if (localSimulationRef != null)
        {
            localSimulationRef.transform.position = position;
        }
    }

    public void SimulateLocal(float dt, Vector3 dir)
    {
        Debug.Log($"SimulateLocal tick count: {Time.frameCount}");
        localPm.SetMoveInput(dir);
        localPm.Tick();
        localPm.particle.Tick(dt);
    }
}
