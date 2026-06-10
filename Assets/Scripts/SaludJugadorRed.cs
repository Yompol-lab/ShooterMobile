using Fusion;
using UnityEngine;

public class SaludJugadorRed : NetworkBehaviour
{
    [Header("Configuración")]
    [Networked] public int Vida { get; set; } = 100;

    private EfectoRagdollRed ragdoll;
    private bool estaMuerto = false;

    public override void Spawned()
    {
        ragdoll = GetComponent<EfectoRagdollRed>();
        if (ragdoll == null) Debug.LogError("¡Te falta el script EfectoRagdollRed en el jugador!");
        estaMuerto = false;
        Vida = 100;
    }

    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TomarDanio(int cantidad, Vector3 posicionDelOrígen)
    {
        if (estaMuerto) return;

        Vida -= cantidad;
        Debug.Log($"Vida restante: {Vida}");

        if (Vida <= 0)
        {
            Vida = 0;
            estaMuerto = true;

          
            Vector3 direccionEmpujon = (transform.position - posicionDelOrígen).normalized;
            
            direccionEmpujon += Vector3.up * 0.5f;

         
            if (ragdoll != null)
            {
                ragdoll.Morir(direccionEmpujon);
            }

            
        }
    }
}