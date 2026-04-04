using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    [SerializeField] PlayerMovementPhysics pm;
    [Networked] public Vector3 serverInputPosition { get; set; }
    
    private ChangeDetector _changeDetector;

    private PredictiveBuffer<Particle2D> test;

    private void Awake()
    {
        //test = new(2);
        pm = GetComponent<PlayerMovementPhysics>();
        //test.InitSimulation(Vector3.zero, pm);
    }

    public override void Spawned()
    {
        //get a change detector from photon
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        //Debug.Log($"[SPAWNED] Object: {Object.Id} HasStateAuthority: {HasStateAuthority} HasInputAuthority: {HasInputAuthority} IsServer: {Runner.IsServer}");
    }

    //this is run every frame so good for detecting visual changes on the client
    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(serverInputPosition):
                    Debug.Log($"[CHANGED] Object: {Object.Id} new pos: {serverInputPosition}");
                    transform.position = serverInputPosition;
                    break;
            }
        }
    }

   
    public override void FixedUpdateNetwork()
    {
        //get the input 
        if (GetInput(out NetworkInputData data))
        {
            pm.SetMoveInput(data.direction);
        }
        
        if (HasStateAuthority)
        {
            float t = Runner.SimulationTime;
            //serverInputPosition = new Vector3(Mathf.Cos(t), Mathf.Sin(t), 0f);
            //update the physics
            pm.Tick();
            //update the particle
            pm.particle.Tick(Runner.DeltaTime);
            serverInputPosition = transform.position;
            Debug.Log($"[HOST] Object: {Object.Id} writing pos: {serverInputPosition}");
        }
    }
}