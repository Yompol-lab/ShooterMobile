using UnityEngine;

public class RadarIcon : MonoBehaviour
{
    [HideInInspector] public RadarPlayer jugador;
    [HideInInspector] public RectTransform mapa;
    [HideInInspector] public Transform radarMin;
    [HideInInspector] public Transform radarMax;

    [Header("Calibración")]
    public float offsetX = 0f;
    public float offsetY = 0f;

    public float escalaX = 1f;
    public float escalaY = 1f;

    [HideInInspector] public float margenX;
    [HideInInspector] public float margenY;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (jugador == null || mapa == null)
            return;

        Vector3 pos = jugador.transform.position;

        float tx = Mathf.InverseLerp(radarMin.position.x, radarMax.position.x, pos.x);
        float tz = Mathf.InverseLerp(radarMin.position.z, radarMax.position.z, pos.z);

        tx = Mathf.Lerp(margenX, 1f - margenX, tx);
        tz = Mathf.Lerp(margenY, 1f - margenY, tz);

        float x = (tx - 0.5f) * mapa.rect.width * escalaX + offsetX;
        float y = (tz - 0.5f) * mapa.rect.height * escalaY + offsetY;

        rect.anchoredPosition = new Vector2(x, y);
    }
}