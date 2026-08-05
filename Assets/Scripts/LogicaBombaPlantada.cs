using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class LogicaBombaPlantada : NetworkBehaviour
{
    public static LogicaBombaPlantada BombaActiva;

    [Header("Configuración Explosión")]
    public float radioExplosion = 15f;
    public float fuerzaExplosion = 80f;
    public int danioMaximo = 500;

    [Header("Efectos")]
    public GameObject efectoVisualExplosion;
    public AudioSource sonidoBeep;
    public AudioClip sonidoExplosion;

    [Header("Configuración Defuse")]
    public float tiempoParaDefusar = 10f;

    [Networked] public float ProgresoDefuse { get; set; }
    [Networked] public NetworkBool EstaSiendoDefusada { get; set; }
    [Networked] public PlayerRef DefuserActual { get; set; }
    [Networked] public NetworkBool FueDefusada { get; set; }

    private float tiempoRestante;
    private bool exploto = false;

    public override void Spawned()
    {
        BombaActiva = this;
        exploto = false;
        FueDefusada = false;
        EstaSiendoDefusada = false;
        ProgresoDefuse = 0f;

        if (sonidoBeep != null) sonidoBeep.Play();
    }

    public override void FixedUpdateNetwork()
    {
        if (exploto || FueDefusada || MatchManager.Instance == null) return;

        if (MatchManager.Instance.EstadoActual == MatchState.BombPlanted)
        {
            
            if (HasStateAuthority && EstaSiendoDefusada)
            {
                ProgresoDefuse += Runner.DeltaTime;

                if (ProgresoDefuse >= tiempoParaDefusar)
                {
                    CompletarDefuse();
                }
            }

           
            tiempoRestante = MatchManager.Instance.TiempoRestante;

            if (sonidoBeep != null)
            {
                sonidoBeep.pitch = Mathf.Lerp(3f, 1f, tiempoRestante / 45f);
            }

            if (tiempoRestante <= 0.1f)
            {
                Explosion();
            }
        }
    }

    [Rpc(RpcSources.InputAuthority | RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void RPC_IntentarDefusar(PlayerRef jugador, NetworkBool iniciar)
    {
        if (FueDefusada || exploto) return;

        if (iniciar)
        {
            if (!EstaSiendoDefusada)
            {
                EstaSiendoDefusada = true;
                DefuserActual = jugador;
                ProgresoDefuse = 0f;
            }
        }
        else
        {
            if (DefuserActual == jugador)
            {
                EstaSiendoDefusada = false;
                DefuserActual = default;
                ProgresoDefuse = 0f;
            }
        }
    }

    private void CompletarDefuse()
    {
        FueDefusada = true;
        EstaSiendoDefusada = false;
        BombaActiva = null;

        if (sonidoBeep != null) sonidoBeep.Stop();

        
        if (MatchManager.Instance != null)
        {
            MatchManager.Instance.RPC_AvisarBombaDefusada(DefuserActual);
        }
    }

    private void Explosion()
    {
        if (exploto || FueDefusada) return;
        exploto = true;
        BombaActiva = null;

        if (sonidoBeep != null) sonidoBeep.Stop();
        if (efectoVisualExplosion != null)
        {
            Instantiate(efectoVisualExplosion, transform.position, Quaternion.identity);
        }

        RPC_ReproducirSonidoExplosion();

        if (HasStateAuthority)
        {
            Collider[] objetosCercanos = Physics.OverlapSphere(transform.position, radioExplosion);
            foreach (Collider hit in objetosCercanos)
            {
                SaludJugadorRed saludJugador = hit.GetComponentInParent<SaludJugadorRed>();
                if (saludJugador != null)
                {
                    float distancia = Vector3.Distance(transform.position, saludJugador.transform.position);
                    float intensidad = Mathf.Clamp01(1f - (distancia / radioExplosion));
                    int danioFinal = Mathf.RoundToInt(intensidad * danioMaximo);
                    saludJugador.RPC_TomarDanio(danioFinal, transform.position);
                }
            }
        }

        Runner.Despawn(Object);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ReproducirSonidoExplosion()
    {
        if (sonidoExplosion != null)
        {
            AudioSource.PlayClipAtPoint(sonidoExplosion, transform.position, 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioExplosion);
    }
}