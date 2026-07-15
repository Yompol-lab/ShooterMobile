using UnityEngine;
using Fusion;
using StarterAssets;

public class RadarManager : MonoBehaviour
{
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

    void Update()
    {
        // Buscar el jugador local
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

        Vector3 pos = jugador.transform.position;

        // Posición normalizada dentro del mapa
        float tx = Mathf.InverseLerp(RadarMin.position.x, RadarMax.position.x, pos.x);
        float tz = Mathf.InverseLerp(RadarMin.position.z, RadarMax.position.z, pos.z);

        // Aplicar márgenes independientes
        tx = Mathf.Lerp(margenX, 1f - margenX, tx);
        tz = Mathf.Lerp(margenY, 1f - margenY, tz);

        // Tamaño del mapa
        float ancho = mapa.rect.width;
        float alto = mapa.rect.height;

        // Convertir a coordenadas del radar
        float x = Mathf.Lerp(-ancho * 0.5f, ancho * 0.5f, tx);
        float y = Mathf.Lerp(-alto * 0.5f, alto * 0.5f, tz);

        // Mover el mapa
        mapa.anchoredPosition = new Vector2(-x, -y);
    }
}