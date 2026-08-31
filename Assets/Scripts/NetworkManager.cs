using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    public NetworkRunner Runner => runner;

    public NetworkRunner runnerPrefab;
    private NetworkRunner runner;

    void Awake()
    {
        Instance = this;
    }

    async void Start()
    {
        runner = Instantiate(runnerPrefab);
        runner.name = "NetworkRunner";

        // Agregamos el reportero de slots de matchmaking
        gameObject.AddComponent<MatchSlotReporter>();

        PlayerJoinSpawner joinSpawner = FindFirstObjectByType<PlayerJoinSpawner>();

        if (joinSpawner != null)
        {
            runner.AddCallbacks(joinSpawner);
        }

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        string roomName = PlayerPrefs.GetString("RoomName", "");

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = string.IsNullOrEmpty(roomName) ? string.Empty : roomName,
            PlayerCount = 10,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
}
