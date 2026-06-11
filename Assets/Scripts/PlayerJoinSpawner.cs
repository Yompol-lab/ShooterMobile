using Fusion;
using Fusion.Sockets;
using UnityEngine;
using System.Collections.Generic;

public class PlayerJoinSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static PlayerJoinSpawner Instance;
    public NetworkPrefabRef playerPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
        
    }

    public void SpawnPlayerFor(PlayerRef player, Team team)
    {
        NetworkRunner runner = NetworkManager.Instance.Runner;

        if (!runner.IsServer)
            return;

        TeamSpawnPoint[] allSpawns = FindObjectsByType<TeamSpawnPoint>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        TeamSpawnPoint[] validSpawns = System.Array.FindAll(
            allSpawns,
            sp => sp.team == team
        );

        Vector3 spawnPos = Vector3.up * 2f;
        Quaternion spawnRot = Quaternion.identity;

        if (validSpawns.Length > 0)
        {
            int randomIndex = Random.Range(0, validSpawns.Length);
            spawnPos = validSpawns[randomIndex].transform.position;
            spawnRot = validSpawns[randomIndex].transform.rotation;
        }

        NetworkObject obj = runner.Spawn(
            playerPrefab,
            spawnPos,
            spawnRot,
            player
        );

        runner.SetPlayerObject(player, obj);
       
    }

    
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}