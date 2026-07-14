using UnityEngine;

public class BotonApuntarUI : MonoBehaviour
{
    public void PresionarApuntar()
    {
        Debug.Log(" PASO 1: El botón de la UI detectó tu dedo.");

        ControladorMira[] jugadores = FindObjectsByType<ControladorMira>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Debug.Log($" PASO 2: Encontré {jugadores.Length} jugadores en la partida que tienen el script ControladorMira.");

        bool encontreAlLocal = false;

        foreach (ControladorMira jugador in jugadores)
        {
            if (jugador.HasStateAuthority)
            {
                encontreAlLocal = true;
                Debug.Log(" PASO 3: ¡Jugador tuyo encontrado! Dando la orden de apuntar...");
                jugador.AlternarMira();
                break;
            }
        }

        if (!encontreAlLocal && jugadores.Length > 0)
        {
            Debug.LogWarning(" ATENCIÓN: Encontré jugadores, pero ninguno tuyo (ninguno tiene StateAuthority).");
        }
    }
}