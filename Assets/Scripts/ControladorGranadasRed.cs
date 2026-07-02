using Fusion;
using UnityEngine;

public enum TipoGranada { Fuego, Humo, Flash, Explosiva }

public class ControladorGranadasRed : NetworkBehaviour
{
    [Header("Prefab de Red de la Granada")]
    public NetworkPrefabRef granadaPrefab;

    [Header("Ajustes de Lanzamiento")]
    public Transform puntoLanzamiento;
    public float fuerzaLanzamiento = 16f;

    [Header("Munición de Utilidad (Sincronizada)")]
    [Networked] public int granadasFuego { get; set; } = 1;
    [Networked] public int granadasHumo { get; set; } = 1;
    [Networked] public int granadasFlash { get; set; } = 2;
    [Networked] public int granadasExplosivas { get; set; } = 1; 

    public void IntentarLanzarGranada(TipoGranada tipo)
    {
        if (tipo == TipoGranada.Fuego && granadasFuego <= 0) return;
        if (tipo == TipoGranada.Humo && granadasHumo <= 0) return;
        if (tipo == TipoGranada.Flash && granadasFlash <= 0) return;
        if (tipo == TipoGranada.Explosiva && granadasExplosivas <= 0) return;

        if (puntoLanzamiento == null) puntoLanzamiento = GetComponentInChildren<Camera>().transform;

        RPC_LanzarGranadaServidor(tipo, puntoLanzamiento.position, puntoLanzamiento.forward);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_LanzarGranadaServidor(TipoGranada tipo, Vector3 posOrigen, Vector3 dirMirado)
    {
        if (tipo == TipoGranada.Fuego && granadasFuego <= 0) return;
        if (tipo == TipoGranada.Humo && granadasHumo <= 0) return;
        if (tipo == TipoGranada.Flash && granadasFlash <= 0) return;
        if (tipo == TipoGranada.Explosiva && granadasExplosivas <= 0) return;

        if (tipo == TipoGranada.Fuego) granadasFuego--;
        if (tipo == TipoGranada.Humo) granadasHumo--;
        if (tipo == TipoGranada.Flash) granadasFlash--;
        if (tipo == TipoGranada.Explosiva) granadasExplosivas--;

        Vector3 posicionSpawn = posOrigen + (dirMirado * 0.5f);
        NetworkObject granadaNet = Runner.Spawn(granadaPrefab, posicionSpawn, Quaternion.identity, Object.InputAuthority);

        if (granadaNet != null)
        {
            GranadaObjetoRed scriptGranada = granadaNet.GetComponent<GranadaObjetoRed>();
            if (scriptGranada != null)
            {
                scriptGranada.ConfigurarTipo(tipo, Object.InputAuthority);
            }

            Rigidbody rb = granadaNet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(dirMirado * fuerzaLanzamiento, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }
    }
}