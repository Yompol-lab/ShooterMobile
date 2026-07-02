using Fusion;
using UnityEngine;

public class GranadaObjetoRed : NetworkBehaviour
{
    public TipoGranada tipoGranada;
    public float tiempoMecha = 2.5f;

    [Header("Prefabs de Efectos Libres (Locales)")]
    public GameObject efectoVisualFuego; 
    public GameObject efectoVisualHumo;   
    public GameObject efectoVisualFlash; 

    private float tiempoDetonacion;
    private bool yaExploto = false;
    private PlayerRef duenioLanzamiento;

    public void ConfigurarTipo(TipoGranada tipo, PlayerRef tirador)
    {
        tipoGranada = tipo;
        duenioLanzamiento = tirador;
        tiempoDetonacion = Time.time + tiempoMecha;
    }

    public override void FixedUpdateNetwork()
    {
        
        if (!Object.HasStateAuthority) return;
        if (yaExploto) return;

        
        if (tipoGranada != TipoGranada.Fuego && Time.time >= tiempoDetonacion)
        {
            DetonarGranada();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!Object.HasStateAuthority) return;
        if (yaExploto) return;

       
        if (tipoGranada == TipoGranada.Fuego)
        {
            DetonarGranada();
        }
    }

    private void DetonarGranada()
    {
        yaExploto = true;
        Vector3 posicionExplosion = transform.position;

     
        RPC_SincronizarEfectosVisuales(posicionExplosion, tipoGranada);

      
        if (tipoGranada == TipoGranada.Flash)
        {
            ProcesarCegueraFlash(posicionExplosion);
        }

        
        Runner.Despawn(Object);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SincronizarEfectosVisuales(Vector3 pos, TipoGranada tipo)
    {
        switch (tipo)
        {
            case TipoGranada.Fuego:
                if (efectoVisualFuego != null) Instantiate(efectoVisualFuego, pos, Quaternion.identity);
                break;
            case TipoGranada.Humo:
                if (efectoVisualHumo != null) Instantiate(efectoVisualHumo, pos, Quaternion.identity);
                break;
            case TipoGranada.Flash:
                if (efectoVisualFlash != null) Instantiate(efectoVisualFlash, pos, Quaternion.identity);
                break;
        }
    }

    private void ProcesarCegueraFlash(Vector3 centroExplosion)
    {
        
        Collider[] impactados = Physics.OverlapSphere(centroExplosion, 18f);

        foreach (Collider col in impactados)
        {
            
            SaludJugadorRed salud = col.GetComponentInParent<SaludJugadorRed>();
            if (salud != null)
            {
               
                Vector3 direccion = (col.transform.position - centroExplosion).normalized;
                float distancia = Vector3.Distance(centroExplosion, col.transform.position);

               
                if (!Physics.Raycast(centroExplosion, direccion, distancia, LayerMask.GetMask("Default", "Map")))
                {
                    
                    salud.RPC_CegarPantallaLocal();
                }
            }
        }
    }
}