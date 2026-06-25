using UnityEngine;
using System.Collections;

public class MunicionArma : MonoBehaviour
{
    public int balasCargador;
    public int balasReserva;
    public bool estaRecargando = false;
    private WeaponData data;

    private Animator miAnimador;
    private bool tieneAnimDisparo = false; 

    public void Configurar(WeaponData armaData)
    {
        data = armaData;
        balasCargador = data.tamañoCargador;
        balasReserva = data.municionReservaMaxima;

        miAnimador = GetComponentInChildren<Animator>();

      
        if (miAnimador != null)
        {
            foreach (AnimatorControllerParameter param in miAnimador.parameters)
            {
                if (param.name == "Disparar" && param.type == AnimatorControllerParameterType.Trigger)
                {
                    tieneAnimDisparo = true;
                }
            }
        }
    }

    public bool IntentarDisparar()
    {
        if (estaRecargando || balasCargador <= 0) return false;

        balasCargador--;

        
        if (miAnimador != null && tieneAnimDisparo)
        {
            miAnimador.SetTrigger("Disparar");
        }

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

        if (miAnimador != null) miAnimador.SetTrigger("Recargar");

        yield return new WaitForSeconds(data.tiempoRecarga);

        int balasFaltantes = data.tamañoCargador - balasCargador;
        int balasARecargar = Mathf.Min(balasFaltantes, balasReserva);

        balasCargador += balasARecargar;
        balasReserva -= balasARecargar;

        estaRecargando = false;
    }
}