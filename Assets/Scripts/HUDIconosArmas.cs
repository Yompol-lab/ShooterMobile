using UnityEngine;
using UnityEngine.UI;

public class HUDIconosArmas : MonoBehaviour
{
    [Header("Referencias a tus Imágenes de la Pantalla")]
    public Image iconoPrincipal;
    public Image iconoSecundaria;
    public Image iconoCuchillo;

    [Header("Efecto Visual (Transparencia)")]
    public Color colorEquipado = new Color(1f, 1f, 1f, 1f); 
    public Color colorGuardado = new Color(1f, 1f, 1f, 0.4f); 

    private PlayerInventory jugadorLocal;

    void Update()
    {
       
        if (jugadorLocal == null)
        {
            BuscarJugadorLocal();
            if (jugadorLocal == null) return; 
        }

       
        ActualizarSlot(iconoPrincipal, jugadorLocal.currentPrimary, jugadorLocal.activeSlot == WeaponSlot.Primary);
        ActualizarSlot(iconoSecundaria, jugadorLocal.currentSecondary, jugadorLocal.activeSlot == WeaponSlot.Secondary);
        ActualizarSlot(iconoCuchillo, jugadorLocal.currentKnife, jugadorLocal.activeSlot == WeaponSlot.Knife);
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

    private void ActualizarSlot(Image imagenUI, GameObject armaObj, bool estaEquipada)
    {
        if (imagenUI == null) return;

       
        if (armaObj == null)
        {
            imagenUI.enabled = false; 
            return;
        }

        
        Weapon scriptArma = armaObj.GetComponent<Weapon>();
        if (scriptArma != null && scriptArma.weaponData != null && scriptArma.weaponData.iconoArma != null)
        {
            imagenUI.sprite = scriptArma.weaponData.iconoArma;
            imagenUI.enabled = true;

           
            imagenUI.color = estaEquipada ? colorEquipado : colorGuardado;

           
            imagenUI.preserveAspect = true;
        }
        else
        {
            imagenUI.enabled = false;
        }
    }
}