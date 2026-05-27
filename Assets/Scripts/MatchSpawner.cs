using Fusion;
using UnityEngine;
using System.Linq;

public class MatchSpawner : NetworkBehaviour
{
    public NetworkPrefabRef playerPrefab;
    public GameObject teamSelectionUI;
    public GameObject crosshairUI;

    public override void Spawned()
    {
        if (teamSelectionUI != null) teamSelectionUI.SetActive(true);
        if (crosshairUI != null) crosshairUI.SetActive(false);
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
        if (teamSelectionUI != null) teamSelectionUI.SetActive(false);
        if (crosshairUI != null) crosshairUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        RPC_Spawn(team, Runner.LocalPlayer);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Spawn(Team team, PlayerRef player)
    {
        TeamSpawnPoint[] allSpawns = FindObjectsByType<TeamSpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var validSpawns = allSpawns.Where(sp => sp.team == team).ToArray();

        Vector3 spawnPos = Vector3.up * 2f;
        Quaternion spawnRot = Quaternion.identity;

        if (validSpawns.Length > 0)
        {
            int randomIndex = Random.Range(0, validSpawns.Length);
            spawnPos = validSpawns[randomIndex].transform.position;
            spawnRot = validSpawns[randomIndex].transform.rotation;
        }

        Runner.Spawn(playerPrefab, spawnPos, spawnRot, player);
    }
}