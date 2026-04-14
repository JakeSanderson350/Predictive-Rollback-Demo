using UnityEngine;

public class ServerSimulationManager : MonoBehaviour
{
    [SerializeField] GameObject serverSimulationPrefab;
    PlayerMovementPhysics serverPm;

    public static ServerSimulationManager instance;

    private GameObject serverSimulationRef;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        serverSimulationRef = Instantiate(serverSimulationPrefab, transform.position, transform.rotation);
        serverPm = serverSimulationRef.GetComponent<PlayerMovementPhysics>();
        instance = this;

        // Sync spawn point
        Particle2D particle = serverPm.particle;
        particle.positionX = (long)(transform.position.x * PhysicsConstants.FP_SCALE);
        particle.positionY = (long)(transform.position.y * PhysicsConstants.FP_SCALE);
    }

    public void SimulateServer(Vector3 serverPos)
    {
        serverSimulationRef.transform.position = serverPos;
        //Debug.Log($"SimulateLocal tick count: {Time.frameCount}");
        //serverPm.SetMoveInput(dir);
        //serverPm.Tick();
        //serverPm.particle.Tick(dt);
    }

    public void SyncWithServer()
    {
        if (serverSimulationRef != null)
        {
            serverSimulationRef.transform.position = transform.position;
        }
    }
}
