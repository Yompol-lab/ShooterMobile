using UnityEngine;

public class RadarIcon : MonoBehaviour
{
    [HideInInspector] public RadarPlayer jugador;
    [HideInInspector] public RectTransform mapa;
    [HideInInspector] public Transform radarMin;
    [HideInInspector] public Transform radarMax;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (jugador == null || mapa == null)
            return;

        Vector3 pos = jugador.transform.position;

        float tx = Mathf.InverseLerp(radarMin.position.x, radarMax.position.x, pos.x);
        float tz = Mathf.InverseLerp(radarMin.position.z, radarMax.position.z, pos.z);

        float ancho = mapa.rect.width;
        float alto = mapa.rect.height;

        float x = Mathf.Lerp(-ancho * 0.5f, ancho * 0.5f, tx);
        float y = Mathf.Lerp(-alto * 0.5f, alto * 0.5f, tz);

        // Como el mapa se mueve, el icono debe compensar ese movimiento
        rect.anchoredPosition = new Vector2(
            x + mapa.anchoredPosition.x,
            y + mapa.anchoredPosition.y
        );
    }
}