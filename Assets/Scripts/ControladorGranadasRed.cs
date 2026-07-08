using Fusion;
using UnityEngine;
public enum TipoGranada { Fuego, Humo, Flash, Explosiva }
public class ControladorGranadasRed : NetworkBehaviour
{
    [Header("Prefabs de Red (Las granadas reales que vuelan y rebotan)")]
    public NetworkPrefabRef prefabGranadaFuego;
    public NetworkPrefabRef prefabGranadaHumo;
    public NetworkPrefabRef prefabGranadaFlash;
    public NetworkPrefabRef prefabGranadaExplosiva;

    [Header("Ajustes de Lanzamiento")]
    public Transform puntoLanzamiento;
    public float fuerzaLanzamiento = 18f;

    [Header("Munición en Mochila (Sincronizada)")]
    [Networked] public int granadasFuego { get; set; } = 0;
    [Networked] public int granadasHumo { get; set; } = 0;
    [Networked] public int granadasFlash { get; set; } = 0;
    [Networked] public int granadasExplosivas { get; set; } = 0;

    public void IntentarLanzarGranada(TipoGranada tipo)
    {
        if (puntoLanzamiento == null) puntoLanzamiento = GetComponentInChildren<Camera>().transform;

      
        RPC_LanzarGranadaServidor(tipo, puntoLanzamiento.position, puntoLanzamiento.forward);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_LanzarGranadaServidor(TipoGranada tipo, Vector3 posOrigen, Vector3 dirMirado)
    {
        NetworkPrefabRef prefabSeleccionado = default;

       
        if (tipo == TipoGranada.Fuego) { if (granadasFuego <= 0) return; granadasFuego--; prefabSeleccionado = prefabGranadaFuego; }
        else if (tipo == TipoGranada.Humo) { if (granadasHumo <= 0) return; granadasHumo--; prefabSeleccionado = prefabGranadaHumo; }
        else if (tipo == TipoGranada.Flash) { if (granadasFlash <= 0) return; granadasFlash--; prefabSeleccionado = prefabGranadaFlash; }
        else if (tipo == TipoGranada.Explosiva) { if (granadasExplosivas <= 0) return; granadasExplosivas--; prefabSeleccionado = prefabGranadaExplosiva; }

        if (prefabSeleccionado.IsValid == false)
        {
            Debug.LogError($" Servidor: El prefab de red para la granada {tipo} no está asignado en el Controlador.");
            return;
        }

       
        Vector3 posicionSpawn = posOrigen + (dirMirado * 0.5f);
        NetworkObject granadaNet = Runner.Spawn(prefabSeleccionado, posicionSpawn, Quaternion.identity, Object.InputAuthority);

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