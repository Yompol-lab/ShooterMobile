using UnityEngine;
using Fusion;

public class RadarPlayer : NetworkBehaviour
{
    [Networked] public NetworkBool visibleEnRadar { get; set; }

    private TickTimer visibleTimer;

    public override void Spawned()
    {
        visibleEnRadar = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            if (visibleEnRadar && visibleTimer.Expired(Runner))
            {
                visibleEnRadar = false;
            }
        }
    }

    public void Revelar(float segundos = 3f)
    {
        if (!HasStateAuthority)
            return;

        visibleEnRadar = true;
        visibleTimer = TickTimer.CreateFromSeconds(Runner, segundos);
    }
}