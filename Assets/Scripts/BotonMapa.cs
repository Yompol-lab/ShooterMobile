using UnityEngine;
using UnityEngine.EventSystems;

public class BotonMapa : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject panelMapa;

    [Header("Objetos a ocultar")]
    public GameObject[] ocultar;

    public void OnPointerDown(PointerEventData eventData)
    {
        panelMapa.SetActive(true);

        foreach (GameObject obj in ocultar)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        panelMapa.SetActive(false);

        foreach (GameObject obj in ocultar)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }
}