using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class MatchmakingScanner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _lobbyRunner;
    private Action<string> _onMatchFound;
    private int _partySize = 1;

    public async void FindMatch(int partySize, Action<string> onMatchFound)
    {
        _partySize = partySize;
        _onMatchFound = onMatchFound;

        _lobbyRunner = gameObject.AddComponent<NetworkRunner>();
        _lobbyRunner.ProvideInput = false;
        _lobbyRunner.AddCallbacks(this);

        var result = await _lobbyRunner.JoinSessionLobby(SessionLobby.Shared);
        
        if (!result.Ok)
        {
            Debug.LogError("Error al conectar al Lobby de Matchmaking");
            GenerateNewRoom();
        }
        else
        {
            Debug.Log("Conectado al Lobby. Esperando lista de sesiones...");
            // Si la lista tarda mucho o está vacía, después de 5 segundos creamos sala nueva.
            Invoke(nameof(TimeoutMatchmaking), 5f);
        }
    }

    private void TimeoutMatchmaking()
    {
        if (_lobbyRunner != null)
        {
            Debug.Log("Timeout de Matchmaking. Creando sala nueva.");
            GenerateNewRoom();
        }
    }

    private void GenerateNewRoom()
    {
        Cleanup();
        string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string codigo = "";
        for (int i = 0; i < 6; i++) codigo += caracteres[UnityEngine.Random.Range(0, caracteres.Length)];
        
        _onMatchFound?.Invoke(codigo);
    }

    private void Cleanup()
    {
        CancelInvoke(nameof(TimeoutMatchmaking));
        if (_lobbyRunner != null)
        {
            _lobbyRunner.RemoveCallbacks(this);
            _lobbyRunner.Shutdown();
            Destroy(_lobbyRunner);
            _lobbyRunner = null;
        }
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Lista de sesiones actualizada. Salas activas: " + sessionList.Count);
        
        string bestSession = null;
        bool foundPriority1 = false;

        foreach (var session in sessionList)
        {
            if (session.Name.StartsWith("PARTY_")) continue; // Ignorar salas de grupo

            int availableSlots = session.MaxPlayers - session.PlayerCount;
            if (availableSlots < _partySize) continue;

            // Revisar Custom Properties para ver slots por equipo
            int ctSlots = 5; // Default si no se reporta
            int tSlots = 5;

            if (session.Properties != null)
            {
                if (session.Properties.TryGetValue("CT", out var ctProp)) ctSlots = ctProp;
                if (session.Properties.TryGetValue("T", out var tProp)) tSlots = tProp;
            }

            // Prioridad 1: Entran todos en un mismo equipo
            if (ctSlots >= _partySize || tSlots >= _partySize)
            {
                bestSession = session.Name;
                foundPriority1 = true;
                break; // Lo mejor que podemos encontrar, salimos
            }
            
            // Prioridad 2: Entran en la sala pero separados
            if (!foundPriority1)
            {
                bestSession = session.Name;
            }
        }

        if (!string.IsNullOrEmpty(bestSession))
        {
            Debug.Log("Partida encontrada: " + bestSession);
            Cleanup();
            _onMatchFound?.Invoke(bestSession);
        }
        else
        {
            Debug.Log("No se encontraron partidas con espacio. Creando sala nueva.");
            GenerateNewRoom();
        }
    }

    // Interfaces vacías de Fusion
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
