using Fusion;
using UnityEngine;

public class EconomiaJugador : NetworkBehaviour
{
    [Header("Billetera")]
    [Networked] public int Dinero { get; set; } = 800;

    public override void Spawned()
    {
        if (HasStateAuthority) Dinero = 800;
    }

    public bool Gastar(int monto)
    {
        if (Dinero >= monto)
        {
            Dinero -= monto;
            return true;
        }
        return false;
    }
}