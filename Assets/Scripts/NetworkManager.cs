using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    public NetworkRunner Runner => runner;

    public NetworkRunner runnerPrefab;
    private NetworkRunner runner;

    async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        runner = Instantiate(runnerPrefab);
        runner.name = "NetworkRunner";

        PlayerJoinSpawner joinSpawner = FindFirstObjectByType<PlayerJoinSpawner>();

        if (joinSpawner != null)
        {
            runner.AddCallbacks(joinSpawner);
        }

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "Sala1",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
}