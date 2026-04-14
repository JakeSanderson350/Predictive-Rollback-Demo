using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Fusion;


public struct InputSnapShot
{
    public Vector2 direction;
    public int tick;
}
public struct PositionSnapshot
{
    public Vector2 position;
    public int tick;
}



public class Player : NetworkBehaviour
{
    [SerializeField] PlayerMovementPhysics pm;
    [Networked] public Vector3 serverInputPosition { get; set; }
    
    private ChangeDetector _changeDetector;

    private List<InputSnapShot> inputSnapShots;
    private List<PositionSnapshot> positionSnapshots;
    
    private void Awake()
    {
        //test = new(2);
        pm = GetComponent<PlayerMovementPhysics>();
        //test.InitSimulation(Vector3.zero, pm);

      
    }

    void Update()
    {
        if (HasInputAuthority)
        {
            transform.position = Vector3.Lerp(LocalSimulationManager.instance.GetLocalPosition(), serverInputPosition, 0.3f);
            //set the position
            //transform.position = LocalSimulationManager.instance.GetLocalPosition();
        }
    }


    public override void Spawned()
    {
        //get a change detector from photon
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        transform.position = serverInputPosition;

        //make sure to set it to the server position
        pm.particle.positionX = (long)(transform.position.x * PhysicsConstants.FP_SCALE);
        pm.particle.positionY = (long)(transform.position.y * PhysicsConstants.FP_SCALE);

        //local player - on client
        if (HasInputAuthority)
        {
            inputSnapShots = new();

            //this needs to be here so that is done on the network correctly
            GetComponent<LocalSimulationManager>().Init();
            GetComponent<ServerSimulationManager>().Init();
        }
        
        //remote player - on client
        if (Object.IsProxy) // !HasInputAuthority && !HasStateAuthority
        {
            positionSnapshots = new(); // never created!
        }
    }

    //this is run every frame so good for detecting visual changes on the client
    public override void Render()
    {
        //todo change this to consume the buffer
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(serverInputPosition):
                    if (HasInputAuthority && !HasStateAuthority)
                    {
                        //reconsiliation
                        Reconciliation(Runner.Tick);
                    }
                    else
                    {
                        if (positionSnapshots != null)
                        {
                            var positionSnap = new PositionSnapshot();
                            positionSnap.position = serverInputPosition;
                            positionSnap.tick = Runner.Tick;

                            positionSnapshots.Add(positionSnap);

                        }
                        
                    }
                    break;
            }
            
        }
        
        
      
        
        //not server or current player
        if (Object.IsProxy)
        {
            //get interploated a positons
            var interpoladedPosition = SampleBuffer(Runner.Tick - 2);
            transform.position = interpoladedPosition;
        }
        
    }

   
    //this can run multiple times per frame
    public override void FixedUpdateNetwork()
    {
        
        //get the input 
        if (GetInput(out NetworkInputData data))
        {
            pm.SetMoveInput(data.direction);

            //only for local player
            if (HasInputAuthority)
            {
              
                var input = new InputSnapShot();
            
                input.direction = data.direction;
                input.tick = Runner.Tick;
            
               
                
                if (Runner.IsForward)
                {
                    inputSnapShots.Add(input);
                    LocalSimulationManager.instance.SimulateLocal(Runner.DeltaTime, data.direction);
                    ServerSimulationManager.instance.SimulateServer(serverInputPosition); // Runner.DeltaTime, data.direction

                }
                
            }
        }
        
        //update server physics
        if (HasStateAuthority)
        {
            //update the physics
            pm.Tick();
            
            //update the particle
            pm.particle.Tick(Runner.DeltaTime);
            serverInputPosition = transform.position;
            
        }
    }

    //find the interpolated position confirmed from the position snapshot
    private Vector3 SampleBuffer(float targetTick)
    {
        
        if ( positionSnapshots == null || positionSnapshots.Count == 0) return transform.position;

        for (int i = 0; i < positionSnapshots.Count - 1; i++)
        {
            if (positionSnapshots[i].tick <= targetTick 
                && positionSnapshots[i + 1].tick >= targetTick)
            {
                //found the correct network position
                var A = positionSnapshots[i];
                var B = positionSnapshots[i + 1];
                //lerp and return lerped position to the user;
        
                // how far between A and B is the target tick?
                float t = (float)(targetTick - A.tick) / (B.tick - A.tick);
                Vector3 result = Vector3.Lerp(A.position, B.position, t);
                
                return result;
            }
        }
        
        
        //missing a position must have been lost!
        return positionSnapshots[positionSnapshots.Count - 1].position;
    }

    //check for the servers position then check that against current position if it is to far hard reset
    private void Reconciliation(float targetTick)
    {
        
        //hard reset
        if (Vector2.Distance(transform.position, serverInputPosition) > 2f)
        {
            transform.position = serverInputPosition;
            LocalSimulationManager.instance.SetCorrection(serverInputPosition);
        }
        else
        {
            return;
        }
        
        //resimulate based off of positon
        for (int i = 0; i < inputSnapShots.Count - 1; i++)
        {
            if (inputSnapShots[i].tick <= targetTick
                && inputSnapShots[i + 1].tick >= targetTick)
            {
                for (int j = i; j < inputSnapShots.Count; j++)
                {
                    pm.SetMoveInput(inputSnapShots[j].direction);
                    pm.Tick();
                    pm.particle.Tick(Runner.DeltaTime);
                }
                break;
            }
           
        }
        
        
        
    }
}