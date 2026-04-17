using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartConnection : MonoBehaviour, INetworkRunnerCallbacks
{
    
    private NetworkRunner _runner;
    
    string name = "EMPTY";
    
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private TMP_InputField inputText;
    [SerializeField] private TMP_Text _text;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    async void StartGame(GameMode mode)
    {
        
        if (mode == GameMode.Host)
        {
            //generate a room name
            name = CreateRoomName().ToString();
            _text.text = name;

        }
        else
        { 
            name = inputText.text;
        }
    
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid) {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

      
        
        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = name,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }


    int CreateRoomName()
    {
        return UnityEngine.Random.Range(0, 10000);
    }

    public void SetRoom(string name)
    {
        this.name = name;
    }
    
    //private void OnGUI()
    //{
    //    if (_runner == null)
    //    {
    //        if (GUI.Button(new Rect(0,0,200,40), "Host"))
    //        {
    //            StartGame(GameMode.Host);
    //        }
    //        if (GUI.Button(new Rect(0,40,200,40), "Join"))
    //        {
    //            StartGame(GameMode.Client);
    //        }
    //    }
    //}

    public void JoinAsHost()
    {
        if (_runner == null)
        {
            StartGame(GameMode.Host);
        }
    }

    public void JoinAsClient()
    {
        if (_runner == null)
        {
            StartGame(GameMode.Client);
        }
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
       
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"OnPlayerJoined - IsServer: {runner.IsServer} player: {player} LocalPlayer: {runner.LocalPlayer}");
    
        if (runner.IsServer)
        {
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(
                _playerPrefab, 
                spawnPosition, 
                Quaternion.identity, 
                player  // ← log what this value is for each spawn
            );
        
            // Mark new object as interesting to all existing players
            foreach (var p in runner.ActivePlayers)
            {
                networkPlayerObject.SetPlayerAlwaysInterested(p, true);
            }

            // Mark ALL existing objects as interesting to the new player
            foreach (var kvp in _spawnedCharacters)
            {
                kvp.Value.SetPlayerAlwaysInterested(player, true);
            }
            Debug.Log($"Spawned {networkPlayerObject.Id} with InputAuthority: {player}");
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector2.up;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector2.down;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector2.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector2.right;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
       
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void SimulateLatency()
    {
        //PhotonNetwork.NetworkingClient.LoadBalancingPeer.IsSimulationEnabled = true;
        //PhotonNetwork.NetworkingClient.LoadBalancingPeer.NetworkSimulationSettings.IncomingLag = 200; // ms
        //PhotonNetwork.NetworkingClient.LoadBalancingPeer.NetworkSimulationSettings.OutgoingLag = 200; // ms
    }

    public void DisconnectPlayer()
    {
        _runner.Shutdown();
    }

}
