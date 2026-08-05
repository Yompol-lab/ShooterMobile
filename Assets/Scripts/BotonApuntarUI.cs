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
        if (jugadorLocal == null || jugadorLocal.Object == null || !jugadorLocal.Object.IsValid)
        {
            BuscarJugadorLocal();
            if (jugadorLocal == null || jugadorLocal.Object == null || !jugadorLocal.Object.IsValid) return;
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
           
            if (j.Object != null && j.Object.IsValid && j.HasStateAuthority)
            {
                jugadorLocal = j;
                break;
            }
        }
    }

    public void PresionarApuntar()
    {

        ControladorMira[] jugadores = FindObjectsByType<ControladorMira>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        bool encontreAlLocal = false;

        foreach (ControladorMira jugador in jugadores)
        {
            
            if (jugador.Object != null && jugador.Object.IsValid && jugador.HasStateAuthority)
            {
                encontreAlLocal = true;
               
                jugador.AlternarMira();
                break;
            }
        }

      
    }
}