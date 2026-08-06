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

    [Header("Efecto Pistola de Agua")]
    public ParticleSystem chorroAgua;

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

        if (!string.IsNullOrEmpty(data.eventoDisparoWwise))
        {
            AkSoundEngine.PostEvent(data.eventoDisparoWwise, gameObject);
        }

        if (miAnimador != null && tieneAnimDisparo)
        {
            miAnimador.SetTrigger("Disparar");
        }

        if (chorroAgua != null)
        {
            chorroAgua.Play();
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

        if (!string.IsNullOrEmpty(data.eventoRecargaWwise))
        {
            AkSoundEngine.PostEvent(data.eventoRecargaWwise, gameObject);
        }

        yield return new WaitForSeconds(data.tiempoRecarga);

        int balasFaltantes = data.tamañoCargador - balasCargador;
        int balasARecargar = Mathf.Min(balasFaltantes, balasReserva);

        balasCargador += balasARecargar;
        balasReserva -= balasARecargar;

        estaRecargando = false;
    }
}