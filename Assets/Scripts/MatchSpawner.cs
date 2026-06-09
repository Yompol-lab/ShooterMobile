using Fusion;
using UnityEngine;
using System.Linq;
using System.Collections;

public class MatchSpawner : MonoBehaviour
{
    public NetworkPrefabRef playerPrefab;
    public GameObject teamSelectionUI;
    public GameObject crosshairUI;

    private bool alreadySpawned = false;

    private IEnumerator Start()
    {
        if (teamSelectionUI != null)
            teamSelectionUI.SetActive(true);

        if (crosshairUI != null)
            crosshairUI.SetActive(false);

        UnlockCursor();

        while (NetworkManager.Instance == null ||
               NetworkManager.Instance.Runner == null ||
               !NetworkManager.Instance.Runner.IsRunning)
        {
            yield return null;
        }

        runnerReady = true;

        Debug.Log("RUNNER PREPARADO");
    }

    public void JoinPolice()
    {
        Debug.Log("BOTON CT APRETADO");
        DoSpawn(Team.Police);
    }

    public void JoinTerrorist()
    {
        Debug.Log("BOTON T APRETADO");
        DoSpawn(Team.Terrorist);
    }
    private bool runnerReady = false;

   
    private void DoSpawn(Team team)
    {
        if (alreadySpawned)
            return;

        if (NetworkManager.Instance == null)
        {
            Debug.LogError("No existe NetworkManager en la escena.");
            return;
        }

        NetworkRunner runner = NetworkManager.Instance.Runner;

        if (!runnerReady)
        {
            Debug.LogError("Esperando conexión...");
            return;
        }

        Debug.Log("RUNNER LISTO");

        TeamSpawnPoint[] allSpawns = FindObjectsByType<TeamSpawnPoint>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        TeamSpawnPoint[] validSpawns = allSpawns
            .Where(sp => sp.team == team)
            .ToArray();

        Vector3 spawnPos = Vector3.up * 2f;
        Quaternion spawnRot = Quaternion.identity;

        if (validSpawns.Length > 0)
        {
            int randomIndex = Random.Range(0, validSpawns.Length);
            spawnPos = validSpawns[randomIndex].transform.position;
            spawnRot = validSpawns[randomIndex].transform.rotation;
        }
        Debug.Log("SPAWN POS = " + spawnPos);

        Debug.Log("IsServer = " + runner.IsServer);
        Debug.Log("IsClient = " + runner.IsClient);
        Debug.Log("LocalPlayer = " + runner.LocalPlayer);

        Debug.Log("Runner IsServer = " + runner.IsServer);

        if (!runner.IsServer)
        {
            Debug.LogWarning("Cliente detectado");

            TeamSelectionRpc rpc = FindFirstObjectByType<TeamSelectionRpc>();

            if (rpc != null)
            {
                rpc.RPC_SelectTeam(
                    runner.LocalPlayer,
                    (int)team
                );
            }

            return;
        }
        Debug.LogError("MATCHSPAWNER SPAWN");
        NetworkObject spawnedPlayer = runner.Spawn(
            playerPrefab,
            spawnPos,
            spawnRot,
            runner.LocalPlayer
        );

        runner.SetPlayerObject(runner.LocalPlayer, spawnedPlayer);

        Debug.Log("PlayerObject asignado: " + runner.LocalPlayer);

        ConnectMobileControls(spawnedPlayer);
        Debug.Log("Jugador eligió equipo: " + team);
        NetworkObject localPlayer =
    NetworkManager.Instance.Runner.GetPlayerObject(
        NetworkManager.Instance.Runner.LocalPlayer);

        Debug.Log("LOCAL PLAYER = " + localPlayer);

        if (localPlayer != null)
        {
            ConnectMobileControls(localPlayer);
        }

        alreadySpawned = true;

        if (teamSelectionUI != null)
            teamSelectionUI.SetActive(false);

        if (crosshairUI != null)
            crosshairUI.SetActive(true);

        UnlockCursor();

        StartCoroutine(ForceUnlockCursor());
    }

    private void ConnectMobileControls(NetworkObject spawnedPlayer)
    {
        if (spawnedPlayer == null)
        {
            Debug.LogError("No se pudo conectar mobile controls porque spawnedPlayer es null.");
            return;
        }

        MobileControlsBridge mobileControls = FindFirstObjectByType<MobileControlsBridge>();

        if (mobileControls == null)
        {
            Debug.LogWarning("No encontré MobileControlsBridge en la escena.");
            return;
        }

        StarterAssets.StarterAssetsInputs inputs = spawnedPlayer.GetComponent<StarterAssets.StarterAssetsInputs>();
        PlayerWeaponController weapon = spawnedPlayer.GetComponent<PlayerWeaponController>();
        PlayerInventory inventory = spawnedPlayer.GetComponent<PlayerInventory>();

        if (inputs == null)
            Debug.LogWarning("El player spawneado no tiene StarterAssetsInputs.");

        if (weapon == null)
            Debug.LogWarning("El player spawneado no tiene PlayerWeaponController.");

        if (inventory == null)
            Debug.LogWarning("El player spawneado no tiene PlayerInventory.");

        mobileControls.starterInputs = inputs;
        mobileControls.weaponController = weapon;
        mobileControls.playerInventory = inventory;

        Debug.Log("MobileControls conectado al player spawneado.");
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator ForceUnlockCursor()
    {
        yield return null;
        UnlockCursor();

        yield return new WaitForSeconds(0.1f);
        UnlockCursor();

        yield return new WaitForSeconds(0.3f);
        UnlockCursor();
    }
}