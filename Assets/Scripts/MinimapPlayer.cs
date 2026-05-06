using UnityEngine;

public class MinimapPlayer : MonoBehaviour
{
    private Transform player; // tu jugador real
    public RectTransform minimap; // imagen del mapa
    public RectTransform icono; // icono del jugador

    public Vector2 worldMin;
    public Vector2 worldMax;
    void Start()
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (obj.GetComponentInChildren<Camera>() != null) 
            {
                player = obj.transform;
                break;
            }
        }

        if (player == null)
        {
            Debug.LogError("No se encontró el jugador local");
        }
    }
    void Update()
    {
        Debug.Log(player.position); 

        Vector3 pos = player.position;

        float x = (pos.x - worldMin.x) / (worldMax.x - worldMin.x);
        float y = (pos.z - worldMin.y) / (worldMax.y - worldMin.y);

        float width = minimap.rect.width;
        float height = minimap.rect.height;

        icono.anchorMin = Vector2.zero;
        icono.anchorMax = Vector2.zero;
        icono.pivot = new Vector2(0.5f, 0.5f);

        icono.anchoredPosition = new Vector2(
            x * width,
            y * height
        );

        icono.localEulerAngles = new Vector3(0, 0, -player.eulerAngles.y);
    }

}
