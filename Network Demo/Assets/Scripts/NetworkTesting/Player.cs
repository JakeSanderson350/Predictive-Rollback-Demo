using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    //private NetworkCharacterController _cc;
    [SerializeField] PlayerMovementPhysics pm;
    [Networked] private Vector3 serverInputPosition { get; set; }

    private void Awake()
    {
        pm = GetComponent<PlayerMovementPhysics>();
    }

    /*public override void FixedUpdateNetwork()
    {
        // 1. Apply input first
        if (GetInput(out NetworkInputData data) && HasStateAuthority)
        {
           
            pm.SetMoveInput(data.direction);
        }

        // 2. Then read/apply position
        if (HasStateAuthority)
        {
            pm.Tick();
            pm.particle.Tick(Runner.DeltaTime);
            serverInputPosition = transform.position;
        }
        else
        {
            transform.position = serverInputPosition;
        }
    }*/
    
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            // Just move in a circle, no physics involved at all
            float t = Runner.SimulationTime;
            serverInputPosition = new Vector3(Mathf.Cos(t), Mathf.Sin(t), 0f);
            transform.position = serverInputPosition;
            Debug.Log($"[HOST] writing pos: {serverInputPosition}");
        }
        else
        {
            transform.position = serverInputPosition;
            Debug.Log($"[CLIENT] reading pos: {serverInputPosition}");
        }
    }
    
}
