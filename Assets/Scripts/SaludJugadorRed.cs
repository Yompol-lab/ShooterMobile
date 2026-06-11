using Fusion;
using UnityEngine;
using StarterAssets;
public class SaludJugadorRed : NetworkBehaviour
{
    [Header("Configuración")]
    [Networked] public int Vida { get; set; } = 100;

    private EfectoRagdollRed ragdoll;
    private bool estaMuerto = false;

    public override void Spawned()
    {
        ragdoll = GetComponent<EfectoRagdollRed>();
        estaMuerto = false;
        Vida = 100; 
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TomarDanio(int cantidad, Vector3 posicionDelOrígen)
    {
        if (estaMuerto) return;

        Vida -= cantidad;
        

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