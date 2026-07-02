using UnityEngine;
using System.Collections.Generic;

public class AreaFuegoDanio : MonoBehaviour
{
    public int danioPorTick = 12;
    public float intervaloDanio = 0.5f; 
    public float tiempoVidaFuego = 7f;  

    private Dictionary<SaludJugadorRed, float> cronometrosJugadores = new Dictionary<SaludJugadorRed, float>();

    private void Start()
    {
        
        Destroy(gameObject, tiempoVidaFuego);
    }

    private void OnTriggerStay(Collider other)
    {
       
        SaludJugadorRed salud = other.GetComponentInParent<SaludJugadorRed>();
        if (salud != null)
        {
            if (!cronometrosJugadores.ContainsKey(salud))
            {
                cronometrosJugadores.Add(salud, 0f);
            }

            
            if (Time.time >= cronometrosJugadores[salud])
            {
                
                salud.RPC_TomarDanio(danioPorTick, transform.position);
                cronometrosJugadores[salud] = Time.time + intervaloDanio;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SaludJugadorRed salud = other.GetComponentInParent<SaludJugadorRed>();
        if (salud != null && cronometrosJugadores.ContainsKey(salud))
        {
            cronometrosJugadores.Remove(salud);
        }
    }
}