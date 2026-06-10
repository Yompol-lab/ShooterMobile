using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class LogicaBombaPlantada : NetworkBehaviour
{
    [Header("Configuración Explosión")]
    public float radioExplosion = 15f; 
    public float fuerzaExplosion = 80f; 
    public int danioMaximo = 500;     

    [Header("Efectos")]
    public GameObject efectoVisualExplosion; 
    public AudioSource sonidoBeep;        
    public AudioClip sonidoExplosion;      

    private float tiempoRestante;
    private bool exploto = false;

    public override void Spawned()
    {
      
        exploto = false;

      
        if (sonidoBeep != null) sonidoBeep.Play();
    }

    public override void FixedUpdateNetwork()
    {
        if (exploto || MatchManager.Instance == null) return;

      
        if (MatchManager.Instance.EstadoActual == MatchState.BombPlanted)
        {
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

    private void Explosion()
    {
        if (exploto) return;
        exploto = true;

        
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