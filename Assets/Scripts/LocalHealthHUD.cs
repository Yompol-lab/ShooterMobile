using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class LocalHealthHUD : MonoBehaviour
{
    [Header("Referencia UI")]
    public TextMeshProUGUI textoVida;
    public Image panelVida;

    // NUEVO: la barra de vida
    public Image barraVida;

    public Color colorCT = Color.blue;
    public Color colorT = Color.red;

    private bool colorAplicado = false;

    void Update()
    {
        var miJugador = FindObjectsByType<SaludJugadorRed>(FindObjectsSortMode.None)
            .FirstOrDefault(j => j.Object != null && j.Object.HasInputAuthority);

        if (miJugador != null)
        {
            // Actualiza el número
            if (textoVida != null)
                textoVida.text = miJugador.Vida.ToString();

            // Actualiza la barra (100 = llena, 50 = mitad, 0 = vacía)
            if (barraVida != null)
                barraVida.fillAmount = 0.5f;

            // Cambia el color una sola vez según el equipo
            if (!colorAplicado)
            {
                ConfiguracionJugadorRed config = miJugador.GetComponent<ConfiguracionJugadorRed>();

                if (config != null)
                {
                    Color color = (config.miEquipo == Team.Police) ? colorCT : colorT;

                    if (panelVida != null)
                        panelVida.color = color;

                    if (barraVida != null)
                        barraVida.color = color;

                    colorAplicado = true;
                }
            }
        }
    }
}