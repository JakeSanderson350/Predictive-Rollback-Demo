using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    //private NetworkCharacterController _cc;
    [SerializeField] PlayerMovementPhysics pm;

    private void Awake()
    {
        pm = GetComponent<PlayerMovementPhysics>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            pm.SetMoveInput(data.direction);
        }
    }
}
