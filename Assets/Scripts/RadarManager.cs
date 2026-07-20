using Fusion;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class RadarManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject iconoCTPrefab;
    public GameObject iconoTPrefab;
    public Transform contenedorIconos;

    [Header("UI")]
    public RectTransform mapa;

    [Header("Límites del mapa")]
    public Transform RadarMin;
    public Transform RadarMax;

    [Header("Margen Horizontal")]
    [Range(0f, 0.45f)]
    public float margenX = 0.04f;

    [Header("Margen Vertical")]
    [Range(0f, 0.45f)]
    public float margenY = 0.18f;

    private FirstPersonController jugador;

    private Dictionary<RadarPlayer, RadarIcon> iconos = new Dictionary<RadarPlayer, RadarIcon>();

    private ConfiguracionJugadorRed miJugador;

    void Update()
    {
        // Buscar jugador local
        if (jugador == null)
        {
            foreach (FirstPersonController p in FindObjectsOfType<FirstPersonController>())
            {
                if (p.HasInputAuthority)
                {
                    jugador = p;
                    break;
                }
            }

            if (jugador == null)
                return;
        }

        // Crear iconos de jugadores
        ActualizarIconos();

        Vector3 pos = jugador.transform.position;

        float tx = Mathf.InverseLerp(RadarMin.position.x, RadarMax.position.x, pos.x);
        float tz = Mathf.InverseLerp(RadarMin.position.z, RadarMax.position.z, pos.z);

        tx = Mathf.Lerp(margenX, 1f - margenX, tx);
        tz = Mathf.Lerp(margenY, 1f - margenY, tz);

        float ancho = mapa.rect.width;
        float alto = mapa.rect.height;

        float x = Mathf.Lerp(-ancho * 0.5f, ancho * 0.5f, tx);
        float y = Mathf.Lerp(-alto * 0.5f, alto * 0.5f, tz);

        mapa.anchoredPosition = new Vector2(-x, -y);
    }

    void ActualizarIconos()
    {
        RadarPlayer[] jugadores = FindObjectsOfType<RadarPlayer>();

        foreach (RadarPlayer rp in jugadores)
        {
            if (rp == null)
                continue;

            if (!iconos.ContainsKey(rp))
            {
                ConfiguracionJugadorRed datos = rp.GetComponent<ConfiguracionJugadorRed>();

                if (datos == null)
                    continue;

                if (miJugador == null && rp.HasInputAuthority)
                    miJugador = datos;

                GameObject prefab = datos.miEquipo == Team.Police ? iconoCTPrefab : iconoTPrefab;

                GameObject nuevo = Instantiate(prefab, mapa);

                RadarIcon icono = nuevo.GetComponent<RadarIcon>();

                icono.jugador = rp;
                icono.mapa = mapa;
                icono.radarMin = RadarMin;
                icono.radarMax = RadarMax;
                

                iconos.Add(rp, icono);
            }
        }
    }
}