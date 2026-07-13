using Fusion;
using UnityEngine;

public class EconomiaJugador : NetworkBehaviour
{
   
    [Networked] public int Dinero { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Dinero = 800; 
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SincronizarPremioRonda(int cantidad)
    {
        if (HasStateAuthority)
        {
            Dinero += cantidad;
            if (Dinero > 16000) Dinero = 16000;

            Debug.Log($" ECONOMÍA: Se acreditaron ${cantidad}. Saldo actual: ${Dinero}");
        }
    }

   
    public bool Gastar(int costo)
    {
        if (Dinero >= costo)
        {
            if (HasStateAuthority)
            {
                Dinero -= costo;
            }
            return true;
        }
        return false;
    }
}