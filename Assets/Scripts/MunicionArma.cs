using UnityEngine;
using System.Collections;

public class MunicionArma : MonoBehaviour
{
    public int balasCargador;
    public int balasReserva;
    public bool estaRecargando = false;

    private WeaponData data;

    
    public void Configurar(WeaponData armaData)
    {
        data = armaData;
        balasCargador = data.tamañoCargador;
        balasReserva = data.municionReservaMaxima;
    }

    
    public bool IntentarDisparar()
    {
        if (estaRecargando || balasCargador <= 0)
        {
           
            return false;
        }

        balasCargador--; 
        Debug.Log($"PUM! Balas: {balasCargador} / {balasReserva}");
        return true; 
    }

    
    public void IniciarRecarga()
    {
        
        if (estaRecargando || balasCargador == data.tamañoCargador || balasReserva <= 0) return;

        StartCoroutine(RutinaRecarga());
    }

    private IEnumerator RutinaRecarga()
    {
        estaRecargando = true;
        Debug.Log("Recargando arma...");

        
        yield return new WaitForSeconds(data.tiempoRecarga);

        int balasFaltantes = data.tamañoCargador - balasCargador;
        int balasARecargar = Mathf.Min(balasFaltantes, balasReserva);

        balasCargador += balasARecargar;
        balasReserva -= balasARecargar;

        estaRecargando = false;
        Debug.Log($"Recarga Lista. Balas: {balasCargador} / {balasReserva}");
    }
}