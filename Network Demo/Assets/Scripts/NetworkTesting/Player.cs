using System.Collections.Generic;
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


    private float timer = 0;
    private const float MAX_TIMER = 2f;

    public bool PredictionEnabled = true;
    public bool ReconciliationEnabled = true;
    public bool InterpolationEnabled = true;
    
    private void Awake()
    {
        //get the local physics movment comp
        pm = GetComponent<PlayerMovementPhysics>();
    }
    
    public override void Spawned()
    {
        //get a change detector from photon
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        //sync to the server position
        transform.position = serverInputPosition;

        //sync the partical postions to the server position
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
        
        //remote player
        if (Object.IsProxy) // !HasInputAuthority && !HasStateAuthority
        {
            positionSnapshots = new(); //creates the snapshot buffer for a remote client
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
                            //read the server position and add it to the buffer
                            var positionSnap = new PositionSnapshot();
                            positionSnap.position = serverInputPosition;
                            positionSnap.tick = Runner.Tick;

                            positionSnapshots.Add(positionSnap);
                        }
                        
                    }
                    
                    timer = 0;
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
        
        //is current palyer lerp the correct positions
        // THIS IS INTERPOLATION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if (HasInputAuthority)
        {
            transform.position = Vector3.Lerp(LocalSimulationManager.instance.GetLocalPosition(), serverInputPosition, 0.2f);
        }

        //read for the host
        if (HasStateAuthority && !HasInputAuthority)
        {
            transform.position = new Vector3(
                pm.particle.positionX / (float)PhysicsConstants.FP_SCALE,
                pm.particle.positionY / (float)PhysicsConstants.FP_SCALE,
                0);
        }


        if (HasInputAuthority && !HasStateAuthority)
        {
            timer += Time.deltaTime;

            if (timer > MAX_TIMER)
            {
                Reconciliation(Runner.Tick);
                timer = 0;
            }
        }
        

        //clean buffers
        DecayPredictionBuffer();
        DecayInputBuffer();
        
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
                
                // THIS IS PREDICTION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
            
            //update the particle --> with no transfrom update
            pm.particle.Tick(Runner.DeltaTime);
            
            //was using the transfrom so the floating points where corrupping --> change to read from teh physics position for no coruption
            serverInputPosition = new Vector3(
                pm.particle.positionX / (float)PhysicsConstants.FP_SCALE,
                pm.particle.positionY / (float)PhysicsConstants.FP_SCALE,
                0);
            
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
        if (!ReconciliationEnabled)
            return;

        //save local postion for reconciliation
        Vector3 localPos = LocalSimulationManager.instance.GetLocalPosition();
        
        //hard reset
        if (Vector2.Distance(localPos, serverInputPosition) > 4f)
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
                    
                    //also resimulate on local
                    LocalSimulationManager.instance.SimulateLocal(Runner.DeltaTime, inputSnapShots[j].direction);
                }
                break;
            }
           
        }
        
        
    }

    private void DecayPredictionBuffer()
    {
        if(positionSnapshots == null || positionSnapshots.Count == 0) return;

        positionSnapshots.RemoveAll( s => s.tick < Runner.Tick - 20);
        
    }

    private void DecayInputBuffer()
    {
        if(inputSnapShots == null || inputSnapShots.Count == 0) return;

        //remove based off of network tick
        inputSnapShots.RemoveAll(s => s.tick < Runner.Tick - 60);
    }
    
}