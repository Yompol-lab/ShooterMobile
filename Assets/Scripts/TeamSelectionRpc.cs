using Fusion;
using UnityEngine;

public class TeamSelectionRpc : NetworkBehaviour
{
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SelectTeam(PlayerRef player, int team)
    {
        Debug.Log("RPC RECIBIDO: " + player + " Team=" + team);

        if (PlayerJoinSpawner.Instance != null)
        {
            PlayerJoinSpawner.Instance.SpawnPlayerFor(
                player,
                (Team)team
            );
        }
    }
}