using Fusion;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public NetworkRunner runnerPrefab;
    private NetworkRunner runner;

    async void Start()
    {
        runner = Instantiate(runnerPrefab);

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "Sala1",
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
}