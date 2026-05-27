using Fusion;
using UnityEngine;
using System.Linq;

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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

        runner.Spawn(playerPrefab, spawnPos, spawnRot, runner.LocalPlayer);

        alreadySpawned = true;

        if (teamSelectionUI != null)
            teamSelectionUI.SetActive(false);

        if (crosshairUI != null)
            crosshairUI.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}