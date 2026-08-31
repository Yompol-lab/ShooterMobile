using TMPro;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.SceneManagement;

public class GroupManager : MonoBehaviour, INetworkRunnerCallbacks
{
    // Variables que podremos usar desde otros scripts
    public static string CodigoGrupo = "";
    public static bool SoyLider = false;
    public static bool EnGrupo = false;

    // Referencias de la UI
    public TextMeshProUGUI txtCodigoGrupo;
    public TextMeshProUGUI txtMiembros;
    public TMP_InputField inputCodigo;

    private NetworkRunner _runner;
    private Dictionary<PlayerRef, string> _partyMembers = new Dictionary<PlayerRef, string>();
    private string _localName;
    public static GroupManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _localName = PlayerPrefs.GetString("NombreJugador", "Jugador_" + UnityEngine.Random.Range(1000, 9999));
    }

    // Crear un grupo
    public async void CrearGrupo()
    {
        string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string codigo = "";

        for (int i = 0; i < 4; i++)
        {
            codigo += caracteres[UnityEngine.Random.Range(0, caracteres.Length)];
        }

        CodigoGrupo = codigo;
        SoyLider = true;
        EnGrupo = true;
        _partyMembers.Clear();

        txtCodigoGrupo.text = "Código: " + codigo;
        txtMiembros.text = "Conectando al grupo...";

        await IniciarSesionDeGrupo("PARTY_" + codigo);
    }

    // Unirse a un grupo
    public async void UnirseGrupo()
    {
        string codigo = inputCodigo.text.Trim().ToUpper();

        if (string.IsNullOrEmpty(codigo))
        {
            Debug.Log("No ingresó ningún código.");
            return;
        }

        CodigoGrupo = codigo;
        SoyLider = false;
        EnGrupo = true;
        _partyMembers.Clear();

        txtCodigoGrupo.text = "Código: " + codigo;
        txtMiembros.text = "Conectando al grupo...";

        await IniciarSesionDeGrupo("PARTY_" + codigo);
    }

    private async System.Threading.Tasks.Task IniciarSesionDeGrupo(string sessionName)
    {
        if (_runner == null)
        {
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = false;
        }

        _runner.AddCallbacks(this);

        var args = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = 4,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        var result = await _runner.StartGame(args);

        if (result.Ok)
        {
            Debug.Log("Conectado a la sala de grupo invisible.");
        }
        else
        {
            txtMiembros.text = "Error al conectar al grupo.";
            EnGrupo = false;
        }
    }

    private void ActualizarUITextoMiembros()
    {
        string texto = "Miembros:\n";
        foreach (var member in _partyMembers.Values)
        {
            texto += "- " + member + "\n";
        }
        txtMiembros.text = texto;
    }

    // --- MÉTODOS DE COMUNICACIÓN EN EL MENÚ ---

    private void EnviarMiNombre(PlayerRef targetPlayer)
    {
        if (_runner == null) return;
        byte[] nameBytes = Encoding.UTF8.GetBytes(_localName);
        byte[] payload = new byte[nameBytes.Length + 1];
        payload[0] = 1; // 1 = Name Message
        Array.Copy(nameBytes, 0, payload, 1, nameBytes.Length);
        
        ReliableKey key = ReliableKey.FromInt(targetPlayer.RawEncoded); // Unique key per player
        _runner.SendReliableDataToPlayer(targetPlayer, key, payload);
    }

    public async void EnviarComandoIrAPartida(string roomCode)
    {
        if (_runner == null || !SoyLider) return;

        byte[] codeBytes = Encoding.UTF8.GetBytes(roomCode);
        byte[] payload = new byte[codeBytes.Length + 1];
        payload[0] = 2; // 2 = GoToMatch Message
        Array.Copy(codeBytes, 0, payload, 1, codeBytes.Length);

        // Send to all other players in the party
        foreach (var player in _runner.ActivePlayers)
        {
            if (player != _runner.LocalPlayer)
            {
                ReliableKey key = ReliableKey.FromInt(player.RawEncoded + 100);
                _runner.SendReliableDataToPlayer(player, key, payload);
            }
        }
        
        // El líder también debe desconectarse e ir
        PlayerPrefs.SetString("RoomName", roomCode);
        PlayerPrefs.Save();
        _runner.RemoveCallbacks(this);
        await _runner.Shutdown();
        Destroy(_runner);
        _runner = null;
        SceneManager.LoadScene("SampleScene");
    }

    private async void RecibirComandoIrAPartida(string roomCode)
    {
        Debug.Log("Recibido comando del líder para ir a la partida: " + roomCode);
        PlayerPrefs.SetString("RoomName", roomCode);
        PlayerPrefs.Save();
        
        // Desconectar de la sala de grupo y cargar la escena de juego
        if (_runner != null)
        {
            _runner.RemoveCallbacks(this);
            await _runner.Shutdown();
            Destroy(_runner);
            _runner = null;
        }

        SceneManager.LoadScene("SampleScene"); // Carga el mapa, NetworkManager hará el resto
    }

    // --- CALLBACKS DE FUSION ---

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            _partyMembers[player] = _localName;
            ActualizarUITextoMiembros();
            
            // Send my name to all already connected players
            foreach(var activePlayer in runner.ActivePlayers)
            {
                if (activePlayer != runner.LocalPlayer) EnviarMiNombre(activePlayer);
            }
        }
        else
        {
            // A remote player joined, send them my name so they know I'm here
            EnviarMiNombre(player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_partyMembers.ContainsKey(player))
        {
            _partyMembers.Remove(player);
            ActualizarUITextoMiembros();
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        if (data.Count == 0) return;

        byte messageType = data.Array[data.Offset];
        
        if (messageType == 1) // Name Message
        {
            string receivedName = Encoding.UTF8.GetString(data.Array, data.Offset + 1, data.Count - 1);
            _partyMembers[player] = receivedName;
            ActualizarUITextoMiembros();
        }
        else if (messageType == 2) // GoToMatch Message
        {
            string roomCode = Encoding.UTF8.GetString(data.Array, data.Offset + 1, data.Count - 1);
            RecibirComandoIrAPartida(roomCode);
        }
    }

    // Interfaces vacías obligatorias
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
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
