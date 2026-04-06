using UnityEngine;

public class LocalSimulationManager : MonoBehaviour
{
    [SerializeField] GameObject localSimulationPrefab;

    public static LocalSimulationManager instance;

    private Player playerRef;

    private GameObject localSimulationRef;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRef = GetComponent<Player>();

        if (playerRef.HasInputAuthority)
        {
            localSimulationRef = Instantiate(localSimulationPrefab, transform.position, transform.rotation);
            instance = this;
        }
    }

    public void SyncLocalWithServer()
    {
        if (localSimulationRef != null)
        {
            localSimulationRef.transform.position = transform.position;
        }
    }
}
