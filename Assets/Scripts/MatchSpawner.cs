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

    private void Start()
    {
        if (teamSelectionUI != null)
            teamSelectionUI.SetActive(true);

        if (crosshairUI != null)
            crosshairUI.SetActive(false);

        UnlockCursor();
    }

    public void JoinPolice()
    {
        DoSpawn(Team.Police);
    }

    public void JoinTerrorist()
    {
        DoSpawn(Team.Terrorist);
    }

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

        if (runner == null || !runner.IsRunning)
        {
            Debug.LogError("El Runner todavía no está activo. Esperá 1 segundo antes de elegir equipo.");
            return;
        }

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

        NetworkObject spawnedPlayer = runner.Spawn(playerPrefab, spawnPos, spawnRot, runner.LocalPlayer);

        ConnectMobileControls(spawnedPlayer);

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