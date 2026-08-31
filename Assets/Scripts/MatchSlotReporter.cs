using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class MatchSlotReporter : MonoBehaviour
{
    public int maxPlayersPerTeam = 5;

    private void Start()
    {
        InvokeRepeating(nameof(UpdateSessionProperties), 2f, 2f);
    }

    private void UpdateSessionProperties()
    {
        if (NetworkManager.Instance == null || NetworkManager.Instance.Runner == null) return;
        
        var runner = NetworkManager.Instance.Runner;
        if (runner.SessionInfo == null || !runner.IsSharedModeMasterClient) return;

        int ctCount = 0;
        int tCount = 0;

        ConfiguracionJugadorRed[] jugadores = FindObjectsByType<ConfiguracionJugadorRed>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var config in jugadores)
        {
            string nombreTeam = config.miEquipo.ToString().ToLower();

            if (nombreTeam.Contains("anti") || nombreTeam.Contains("counter") || nombreTeam == "ct" || nombreTeam.Contains("policia") || config.miEquipo == Team.Police)
                ctCount++;
            else if (nombreTeam.Contains("terror") || nombreTeam == "t" || config.miEquipo == Team.Terrorist)
                tCount++;
        }

        int ctSlots = Mathf.Max(0, maxPlayersPerTeam - ctCount);
        int tSlots = Mathf.Max(0, maxPlayersPerTeam - tCount);

        var props = new Dictionary<string, SessionProperty>();
        props["CT"] = ctSlots;
        props["T"] = tSlots;

        runner.SessionInfo.UpdateCustomProperties(props);
    }
}
