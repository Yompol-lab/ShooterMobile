using Fusion;
using UnityEngine;
using System.Collections;

public class DummyEntrenamiento : NetworkBehaviour
{
    [Header("Configuración del Dummy")]
    public int vidaMaxima = 100;

    [Networked] public int vidaActual { get; set; }
    [Networked] public bool estaMuerto { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            vidaActual = vidaMaxima;
            estaMuerto = false;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TomarDanio(int cantidad, Vector3 posicionDelOrigen)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            estaMuerto = true;

            StartCoroutine(RutinaRespawnDummy());
        }
    }

    private IEnumerator RutinaRespawnDummy()
    {
        RPC_ActualizarVisual(false);

       
        yield return new WaitForSeconds(3f);

        vidaActual = vidaMaxima;
        estaMuerto = false;

        RPC_ActualizarVisual(true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ActualizarVisual(bool mostrar)
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = mostrar;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(mostrar);
        }
    }
}