using UnityEngine;

public class RadarIcon : MonoBehaviour
{
    [HideInInspector] public RadarPlayer jugador;
    [HideInInspector] public RectTransform mapa;
    [HideInInspector] public Transform radarMin;
    [HideInInspector] public Transform radarMax;

    [HideInInspector] public float margenX;
    [HideInInspector] public float margenY;

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

        tx = Mathf.Lerp(margenX, 1f - margenX, tx);
        tz = Mathf.Lerp(margenY, 1f - margenY, tz);

        float ancho = mapa.rect.width;
        float alto = mapa.rect.height;

        float x = Mathf.Lerp(-ancho * 0.5f, ancho * 0.5f, tx);
        float y = Mathf.Lerp(-alto * 0.5f, alto * 0.5f, tz);

        rect.anchoredPosition = new Vector2(-x, -y);
    }
}