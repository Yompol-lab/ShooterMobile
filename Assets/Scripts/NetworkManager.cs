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

        //FusionPlayerSpawner spawner = FindFirstObjectByType<FusionPlayerSpawner>();

        //if (spawner != null)
        //{
        //    runner.AddCallbacks(spawner);
        //    Debug.Log("FusionPlayerSpawner registrado.");
        //}
        //else
        //{
        //    Debug.LogError("No encontré FusionPlayerSpawner.");
        //}

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "Sala1",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log("Fusion conectado correctamente.");
            Debug.Log("IsServer = " + runner.IsServer);
            Debug.Log("IsClient = " + runner.IsClient);
            Debug.Log("LocalPlayer = " + runner.LocalPlayer);
        }
        else
        {
            Debug.LogError("Fusion no pudo iniciar: " + result.ShutdownReason);
        }
    }
}