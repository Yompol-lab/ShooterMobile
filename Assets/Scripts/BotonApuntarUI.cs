using UnityEngine;
using UnityEngine.UI; 

public class BotonApuntarUI : MonoBehaviour
{
    [Header("Configuración AWP")]
    [Tooltip("El nombre exacto del arma en tu WeaponData (ej: AWP, Sniper)")]
    public string nombreDelAWP = "AWP";

    private Image imagenBoton;
    private Button boton;
    private PlayerInventory jugadorLocal;

    private void Start()
    {
       
        imagenBoton = GetComponent<Image>();
        boton = GetComponent<Button>();
    }

    private void Update()
    {
        
        if (jugadorLocal == null)
        {
            BuscarJugadorLocal();
            if (jugadorLocal == null) return;
        }

      
        bool tieneAWP = false;

        
        if (jugadorLocal.activeSlot == WeaponSlot.Primary)
        {
            GameObject armaActiva = jugadorLocal.GetActiveWeaponObject();
            if (armaActiva != null)
            {
                Weapon scriptArma = armaActiva.GetComponent<Weapon>();
                if (scriptArma != null && scriptArma.weaponData != null)
                {
                    
                    if (scriptArma.weaponData.weaponName.ToUpper().Contains(nombreDelAWP.ToUpper()))
                    {
                        tieneAWP = true;
                    }
                }
            }
        }

       
        if (imagenBoton != null) imagenBoton.enabled = tieneAWP;
        if (boton != null) boton.interactable = tieneAWP;
    }

    private void BuscarJugadorLocal()
    {
        PlayerInventory[] jugadores = FindObjectsByType<PlayerInventory>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (PlayerInventory j in jugadores)
        {
            if (j.Object != null && j.HasStateAuthority)
            {
                jugadorLocal = j;
                break;
            }
        }
    }

   
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