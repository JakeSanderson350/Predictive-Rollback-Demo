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

    public override void FixedUpdateNetwork()
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
            serverInputPosition = transform.position;
        }
        else
        {
            transform.position = serverInputPosition;
        }
    }
    
}
