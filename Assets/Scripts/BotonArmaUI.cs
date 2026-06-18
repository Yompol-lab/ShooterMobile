using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotonArmaUI : MonoBehaviour
{
    [Header("Datos del Arma")]
    public WeaponData armaAsignada; 

    [Header("Referencias Visuales del Botón")]
    public TextMeshProUGUI textoNombre; 
    public TextMeshProUGUI textoPrecio; 

    void Start()
    {
        
        if (armaAsignada == null)
        {
            gameObject.SetActive(false);
            return;
        }

        
        if (textoNombre != null) textoNombre.text = armaAsignada.weaponName;
        if (textoPrecio != null) textoPrecio.text = "$ " + armaAsignada.precio.ToString();

        
        Button miBoton = GetComponent<Button>();
        if (miBoton != null)
        {
            miBoton.onClick.AddListener(EjecutarCompra);
        }
    }

    private void EjecutarCompra()
    {
        
        TiendaManager manager = FindFirstObjectByType<TiendaManager>();
        if (manager != null)
        {
            manager.ComprarArma(armaAsignada);
        }
    }
}