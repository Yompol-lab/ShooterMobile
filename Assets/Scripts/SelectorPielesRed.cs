using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class SelectorPielesRed : NetworkBehaviour
{
    [Header("El Esqueleto Maestro")]
    public Transform rigPrincipal; 

    [Header("La malla de tu soldado por defecto")]
    public GameObject mallaOriginal;

    [Header("Los otros personajes COMPLETOS")]
    public GameObject[] personajesExtra; 

    [Networked] public int indicePiel { get; set; }
    private ChangeDetector changeDetector;

   
    private class ParDeHuesos { public Transform huesoEsclavo; public Transform huesoMaestro; }
    private List<ParDeHuesos> huesosSincronizados = new List<ParDeHuesos>();

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        PrepararMimos();

        if (HasStateAuthority)
        {
            ConfiguracionJugadorRed miConfig = GetComponent<ConfiguracionJugadorRed>();
            if (miConfig != null)
            {
              
                indicePiel = (miConfig.miEquipo == Team.Terrorist) ? 1 : 0;
            }
        }
        ActualizarPiel();
    }

    private void PrepararMimos()
    {
        huesosSincronizados.Clear();
        Transform[] huesosMaestros = rigPrincipal.GetComponentsInChildren<Transform>(true);

        foreach (GameObject personaje in personajesExtra)
        {
            if (personaje == null) continue;

            Transform[] huesosEsclavos = personaje.GetComponentsInChildren<Transform>(true);

            foreach (Transform esclavo in huesosEsclavos)
            {
                string nombreBuscado = LimpiarNombreHueso(esclavo.name);

                foreach (Transform maestro in huesosMaestros)
                {
                    if (LimpiarNombreHueso(maestro.name) == nombreBuscado)
                    {
                       
                        huesosSincronizados.Add(new ParDeHuesos { huesoEsclavo = esclavo, huesoMaestro = maestro });
                        break;
                    }
                }
            }
        }
    }

    private string LimpiarNombreHueso(string nombreOriginal)
    {
        if (string.IsNullOrEmpty(nombreOriginal)) return "";
        if (nombreOriginal.Contains(":")) return nombreOriginal.Substring(nombreOriginal.LastIndexOf(':') + 1);
        return nombreOriginal;
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            if (change == nameof(indicePiel))
            {
                ActualizarPiel();
            }
        }
    }

    private void ActualizarPiel()
    {
      
        if (mallaOriginal != null)
        {
            mallaOriginal.SetActive(indicePiel == 0);
        }

        
        for (int i = 0; i < personajesExtra.Length; i++)
        {
            if (personajesExtra[i] != null)
            {
                personajesExtra[i].SetActive(indicePiel == i + 1);
            }
        }
    }

    private void LateUpdate()
    {
       
        if (indicePiel > 0) 
        {
            foreach (ParDeHuesos par in huesosSincronizados)
            {
               
                if (par.huesoEsclavo.gameObject.activeInHierarchy)
                {
                    par.huesoEsclavo.position = par.huesoMaestro.position;
                    par.huesoEsclavo.rotation = par.huesoMaestro.rotation;
                }
            }
        }
    }
}