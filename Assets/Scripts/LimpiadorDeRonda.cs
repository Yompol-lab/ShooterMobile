using Fusion;
using UnityEngine;

public class LimpiadorDeRonda : NetworkBehaviour
{
    private MatchState estadoAnterior;

    public override void FixedUpdateNetwork()
    {
        
        if (!Object.HasStateAuthority) return;

        if (MatchManager.Instance == null) return;

        MatchState estadoActual = MatchManager.Instance.EstadoActual;

        
        if (estadoActual == MatchState.BuyTime && estadoAnterior != MatchState.BuyTime)
        {
            LimpiarArmasDelSuelo();
        }

        estadoAnterior = estadoActual;
    }

    private void LimpiarArmasDelSuelo()
    {
        
        ArmaEnElPisoRed[] armasEnSuelo = FindObjectsByType<ArmaEnElPisoRed>(FindObjectsSortMode.None);

        foreach (ArmaEnElPisoRed arma in armasEnSuelo)
        {
            
            if (arma.transform.parent == null)
            {
                NetworkObject no = arma.GetComponent<NetworkObject>();
                if (no != null)
                {
                   
                    Runner.Despawn(no);
                }
            }

            
            if (arma.gameObject.name.Contains("C4") || arma.gameObject.name.Contains("Bomba")) continue;

        }
    }
}