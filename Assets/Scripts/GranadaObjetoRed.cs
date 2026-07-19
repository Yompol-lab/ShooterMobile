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
    public GameObject efectoVisualExplosion;

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
      
        if (Object == null || !Object.IsValid || !Object.HasStateAuthority) return;
        if (yaExploto) return;

        if (tipoGranada != TipoGranada.Fuego && Time.time >= tiempoDetonacion)
        {
            DetonarGranada();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        if (Object == null || !Object.IsValid || !Object.HasStateAuthority) return;
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
        else if (tipoGranada == TipoGranada.Explosiva)
        {
            ProcesarDanioExplosion(posicionExplosion);
        }

        Runner.Despawn(Object);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SincronizarEfectosVisuales(Vector3 pos, TipoGranada tipo)
    {
        switch (tipo)
        {
            case TipoGranada.Fuego:
                if (efectoVisualFuego != null)
                {
                    GameObject fuegoObj = Instantiate(efectoVisualFuego, pos, Quaternion.identity);
                   
                }
                break;

            case TipoGranada.Humo:
                if (efectoVisualHumo != null)
                {
                    
                    GameObject humoObj = Instantiate(efectoVisualHumo, pos, Quaternion.identity);
                    Destroy(humoObj, 15f);
                }
                break;

            case TipoGranada.Flash:
                if (efectoVisualFlash != null)
                {
                    GameObject flashObj = Instantiate(efectoVisualFlash, pos, Quaternion.identity);
                    Destroy(flashObj, 3f);
                }
                break;

            case TipoGranada.Explosiva:
                if (efectoVisualExplosion != null)
                {
                    GameObject expObj = Instantiate(efectoVisualExplosion, pos, Quaternion.identity);
                    Destroy(expObj, 5f);
                }
                break;
        }
    }

    private void ProcesarDanioExplosion(Vector3 centroExplosion)
    {
        float radioMaximo = 12f;
        float danioMaximo = 75f; 

        Collider[] impactados = Physics.OverlapSphere(centroExplosion, radioMaximo);
        foreach (Collider col in impactados)
        {
            SaludJugadorRed salud = col.GetComponentInParent<SaludJugadorRed>();
            if (salud != null)
            {
                Vector3 direccion = (col.transform.position - centroExplosion).normalized;
                float distancia = Vector3.Distance(centroExplosion, col.transform.position);

              
                if (!Physics.Raycast(centroExplosion, direccion, distancia, LayerMask.GetMask("Default", "Map")))
                {
                   
                    float intensidad = Mathf.Clamp01(1f - (distancia / radioMaximo));
                    int danioFinal = Mathf.RoundToInt(intensidad * danioMaximo);
                    salud.RPC_TomarDanio(danioFinal, centroExplosion);
                }
            }
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
               
                Vector3 dirAlJugador = (col.transform.position - centroExplosion).normalized;
                float distancia = Vector3.Distance(centroExplosion, col.transform.position);

                if (!Physics.Raycast(centroExplosion, dirAlJugador, distancia, LayerMask.GetMask("Default", "Map")))
                {
                   
                    Vector3 dirMiradaJugador = col.transform.forward;
                   
                    Vector3 dirHaciaFlash = -dirAlJugador;

                    
                    float anguloVision = Vector3.Angle(dirMiradaJugador, dirHaciaFlash);

                   
                    if (anguloVision < 75f)
                    {
                        salud.RPC_CegarPantallaLocal();
                    }
                }
            }
        }
    }
}