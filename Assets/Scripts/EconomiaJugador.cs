using Fusion;
using UnityEngine;

public class EconomiaJugador : NetworkBehaviour
{
    [Header("Configuración Inicial")]
    public int dineroInicial = 800; 

    [Header("Billetera Actual (Red)")]
    [Networked] public int Dinero { get; set; }

    public override void Spawned()
    {
        
        if (HasStateAuthority)
        {
            Dinero = dineroInicial;
        }
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