using Fusion;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public NetworkRunner runnerPrefab;
    private NetworkRunner runner;

    public NetworkPrefabRef playerPrefab;

    async void Start()
    {
        runner = Instantiate(runnerPrefab);

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "Sala1",
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        
        await System.Threading.Tasks.Task.Delay(500);

        
        if (VivoxManager.Instance != null)
        {
            VivoxManager.Instance.Login();
            VivoxManager.Instance.JoinVoice("team");
        }

        SpawnPlayer();
    }

    async void SpawnPlayer()
    {
        while (runner.LocalPlayer == default)
            await System.Threading.Tasks.Task.Delay(100);

        if (runner.IsServer)
        {
            runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, runner.LocalPlayer);
        }
    }
}