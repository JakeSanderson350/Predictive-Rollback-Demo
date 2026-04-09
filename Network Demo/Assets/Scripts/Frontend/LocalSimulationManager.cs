using UnityEngine;

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
    }

    public void SetCorrection(Vector3 position)
    {
        localSimulationRef.transform.position = position;
    }

    public void SyncLocalWithServer()
    {
        if (localSimulationRef != null)
        {
            localSimulationRef.transform.position = transform.position;
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
