using UnityEngine;

public class DebugButtonController : MonoBehaviour
{
    public void Sync()
    {
        LocalSimulationManager.instance.SyncLocalWithServer();
    }
}
