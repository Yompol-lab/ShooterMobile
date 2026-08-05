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

    [Header("Calibración de Iconos")]
    public float escalaIconosX = 1f;
    public float escalaIconosY = 1f;

    public float offsetIconosX = 0f;
    public float offsetIconosY = 0f;

    private FirstPersonController jugador;

    private Dictionary<RadarPlayer, RadarIcon> iconos = new Dictionary<RadarPlayer, RadarIcon>();

    private ConfiguracionJugadorRed miJugador;

    void Update()
    {
      
        if (jugador == null || jugador.Object == null || !jugador.Object.IsValid)
        {
            FirstPersonController[] todosLosJugadores = FindObjectsByType<FirstPersonController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (FirstPersonController p in todosLosJugadores)
            {
                if (p.Object != null && p.Object.IsValid && p.HasInputAuthority)
                {
                    jugador = p;
                    break;
                }
            }

            if (jugador == null) return;
        }

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

        foreach (var par in iconos)
        {
            RadarPlayer rp = par.Key;
            RadarIcon icono = par.Value;

            if (rp == null || icono == null) continue;

            ConfiguracionJugadorRed datos = rp.GetComponent<ConfiguracionJugadorRed>();

            if (datos == null || datos.Object == null || !datos.Object.IsValid) continue;
            if (miJugador == null || miJugador.Object == null || !miJugador.Object.IsValid) continue;

            bool esAliado = datos.miEquipo == miJugador.miEquipo;

            if (esAliado)
                icono.gameObject.SetActive(true);
            else
                icono.gameObject.SetActive(rp.visibleEnRadar);
        }
    }

    void ActualizarIconos()
    {
        RadarPlayer[] jugadores = FindObjectsByType<RadarPlayer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (RadarPlayer rp in jugadores)
        {
            if (rp == null || rp.Object == null || !rp.Object.IsValid) continue;

            if (!iconos.ContainsKey(rp))
            {
                ConfiguracionJugadorRed datos = rp.GetComponent<ConfiguracionJugadorRed>();

                if (datos == null || datos.Object == null || !datos.Object.IsValid) continue;

                if (miJugador == null && rp.HasInputAuthority) miJugador = datos;

                GameObject prefab = datos.miEquipo == Team.Police ? iconoCTPrefab : iconoTPrefab;

                GameObject nuevo = Instantiate(prefab, contenedorIconos);
                RadarIcon icono = nuevo.GetComponent<RadarIcon>();

                icono.jugador = rp;
                icono.mapa = contenedorIconos as RectTransform;
                icono.radarMin = RadarMin;
                icono.radarMax = RadarMax;

                icono.margenX = margenX;
                icono.margenY = margenY;

                icono.escalaX = escalaIconosX;
                icono.escalaY = escalaIconosY;

                icono.offsetX = offsetIconosX;
                icono.offsetY = offsetIconosY;

                iconos.Add(rp, icono);
            }
        }
    }
}