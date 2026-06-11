using UnityEngine;
using TMPro;
using System.Linq;

public class LocalHealthHUD : MonoBehaviour
{
    [Header("Referencia UI")]
    public TextMeshProUGUI textoVida;

    void Update()
    {
        
        var miJugador = FindObjectsByType<SaludJugadorRed>(FindObjectsSortMode.None)
            .FirstOrDefault(j => j.Object != null && j.Object.HasInputAuthority);

        if (miJugador != null && textoVida != null)
        {
            
            textoVida.text = miJugador.Vida.ToString();
        }
    }
}