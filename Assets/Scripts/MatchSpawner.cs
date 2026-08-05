using Fusion;
using UnityEngine;
using System.Linq;
using System.Collections;

public class MatchSpawner : MonoBehaviour
{
    public NetworkPrefabRef playerPrefab;

    public GameObject teamSelectionUI;
    public GameObject crosshairUI;
    public GameObject panelVida;

    private bool alreadySpawned = false;
    private bool runnerReady = false;

    private IEnumerator Start()
    {
        if (teamSelectionUI != null)
            teamSelectionUI.SetActive(true);

        if (crosshairUI != null)
            crosshairUI.SetActive(false);

        if (panelVida != null)
            panelVida.SetActive(false);

        UnlockCursor();

        while (NetworkManager.Instance == null ||
               NetworkManager.Instance.Runner == null ||
               !NetworkManager.Instance.Runner.IsRunning)
        {
            yield return null;
        }

        runnerReady = true;
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
        if (alreadySpawned || NetworkManager.Instance == null || !runnerReady)
        {
            Debug.LogWarning("Tranquilo fiera, el servidor todavía está conectando...");
            return;
        }

        NetworkRunner runner = NetworkManager.Instance.Runner;

        if (runner == null || !runner.IsRunning) return;

        alreadySpawned = true;

        try
        {
            if (teamSelectionUI != null)
                teamSelectionUI.SetActive(false);

            if (crosshairUI != null)
                crosshairUI.SetActive(true);

            if (panelVida != null)
                panelVida.SetActive(true);

            if (PlayerUIManager.Instance != null)
            {
                PlayerUIManager.Instance.MostrarLista();
            }

            UnlockCursor();
            StartCoroutine(ForceUnlockCursor());

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
                spawnPos = validSpawns[randomIndex].transform.position + (Vector3.up * 2f);
                spawnRot = validSpawns[randomIndex].transform.rotation;
            }

            NetworkObject spawnedPlayer = runner.Spawn(
                playerPrefab,
                spawnPos,
                spawnRot,
                runner.LocalPlayer,
                (rn, obj) =>
                {
                    ConfiguracionJugadorRed config = obj.GetComponent<ConfiguracionJugadorRed>();

                    if (config != null)
                    {
                        config.miEquipo = team;
                    }
                }
            );

            runner.SetPlayerObject(runner.LocalPlayer, spawnedPlayer);
        }
        catch (System.Exception ex)
        {
            alreadySpawned = false;

            if (teamSelectionUI != null) teamSelectionUI.SetActive(true);

            if (crosshairUI != null) crosshairUI.SetActive(false);
            if (panelVida != null) panelVida.SetActive(false);
        }
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