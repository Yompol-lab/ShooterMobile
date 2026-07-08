using UnityEngine;
using StarterAssets;
using TMPro;

public class MobileControlsBridge : MonoBehaviour
{
    [Header("Joysticks")]
    public Joystick movementJoystick;
    public Joystick cameraJoystick;

    [Header("Player")]
    public StarterAssetsInputs starterInputs;
    public PlayerWeaponController weaponController;
    public PlayerInventory playerInventory;

    [Header("Camara")]
    public float cameraSensitivity = 2f;
    public bool invertY = false;

    [Header("UI Pantalla")]
    public TextMeshProUGUI textoMunicionHUD;

    [HideInInspector] public ConfiguracionJugadorRed jugadorLocal;

    [Header("Efecto Flashbang UI")]
    public CanvasGroup fondoBlancoFlash;

    
    private PlayerInventory GetInv()
    {
        if (playerInventory != null) return playerInventory;
        if (jugadorLocal != null) return jugadorLocal.GetComponent<PlayerInventory>();
        return null;
    }

    
    public void BotonEquiparFuego() { EquiparGranada(WeaponSlot.Fuego); }
    public void BotonEquiparHumo() { EquiparGranada(WeaponSlot.Humo); }
    public void BotonEquiparFlash() { EquiparGranada(WeaponSlot.Flash); }
    public void BotonEquiparExplosiva() { EquiparGranada(WeaponSlot.Explosiva); }

    private void EquiparGranada(WeaponSlot slot)
    {
        PlayerInventory inv = GetInv();

        if (inv != null)
        {
            ControladorGranadasRed control = inv.GetComponent<ControladorGranadasRed>();
            if (control != null)
            {
                bool tieneMunicion = false;
                if (slot == WeaponSlot.Fuego && control.granadasFuego > 0) tieneMunicion = true;
                else if (slot == WeaponSlot.Humo && control.granadasHumo > 0) tieneMunicion = true;
                else if (slot == WeaponSlot.Flash && control.granadasFlash > 0) tieneMunicion = true;
                else if (slot == WeaponSlot.Explosiva && control.granadasExplosivas > 0) tieneMunicion = true;

                if (tieneMunicion)
                {
                    inv.EquipSlot(slot);
                    inv.RPC_SincronizarSlotRed(slot);
                    Debug.Log(" UI: Intentando equipar " + slot.ToString());
                }
                else
                {
                    Debug.LogWarning(" UI: Tocaste el botón pero no tenés munición de esta granada.");
                }
            }
        }
        else
        {
            Debug.LogError(" UI: El Canvas no encuentra a tu jugador local.");
        }
    }

    public System.Collections.IEnumerator RutinaEfectoFlash()
    {
        if (fondoBlancoFlash == null) yield break;
        fondoBlancoFlash.alpha = 1f;
        yield return new WaitForSeconds(2f);
        while (fondoBlancoFlash.alpha > 0f)
        {
            fondoBlancoFlash.alpha -= Time.deltaTime * 0.7f;
            yield return null;
        }
    }

    public void BotonRecargarUI()
    {
        PlayerInventory inv = GetInv();
        if (inv != null)
        {
            GameObject armaObj = inv.GetActiveWeaponObject();
            if (armaObj != null)
            {
                MunicionArma mun = armaObj.GetComponent<MunicionArma>();
                if (mun != null) mun.IniciarRecarga();
            }
        }
    }

    public void BotonTirarArma()
    {
        PlayerInventory inv = GetInv();
        if (inv != null) inv.BotonTirarArma();
    }

    public void BotonSacarBomba()
    {
        PlayerInventory inv = GetInv();
        if (inv != null)
        {
            if (inv.activeSlot == WeaponSlot.Bomb && inv.enZonaPlantar)
            {
                inv.PlantarBomba();
            }
            else if (inv.activeSlot == WeaponSlot.Bomb)
            {
                inv.EquipSlot(WeaponSlot.Knife);
                inv.RPC_SincronizarSlotRed(WeaponSlot.Knife);
            }
            else
            {
                inv.EquipSlot(WeaponSlot.Bomb);
                inv.RPC_SincronizarSlotRed(WeaponSlot.Bomb);
            }
        }
    }

    private void Update()
    {
        PlayerInventory inv = GetInv();
        if (textoMunicionHUD != null && inv != null)
        {
            GameObject armaObj = inv.GetActiveWeaponObject();
            if (armaObj != null)
            {
                MunicionArma mun = armaObj.GetComponent<MunicionArma>();
                if (mun != null)
                {
                    if (mun.estaRecargando) textoMunicionHUD.text = "Recargando";
                    else textoMunicionHUD.text = $"{mun.balasCargador} / {mun.balasReserva}";
                }
                else textoMunicionHUD.text = "";
            }
            else textoMunicionHUD.text = "";
        }

        if (starterInputs == null) return;

        if (movementJoystick != null)
        {
            Vector2 moveInput = new Vector2(movementJoystick.Horizontal, movementJoystick.Vertical);
            starterInputs.MoveInput(moveInput);
        }

        if (cameraJoystick != null)
        {
            float lookX = cameraJoystick.Horizontal;
            float lookY = cameraJoystick.Vertical * -1f;
            if (invertY) lookY *= -1f;
            Vector2 lookInput = new Vector2(lookX, lookY) * cameraSensitivity;
            starterInputs.LookInput(lookInput);
        }
    }

    public void FireButtonDown() { if (weaponController != null) weaponController.MobileFireDown(); }
    public void FireButtonUp() { if (weaponController != null) weaponController.MobileFireUp(); }
    public void JumpButtonDown() { if (starterInputs != null) starterInputs.JumpInput(true); }
    public void JumpButtonUp() { if (starterInputs != null) starterInputs.JumpInput(false); }

    public void EquipPrimaryButton() { PlayerInventory inv = GetInv(); if (inv != null) { inv.EquipSlot(WeaponSlot.Primary); inv.RPC_SincronizarSlotRed(WeaponSlot.Primary); } }
    public void EquipSecondaryButton() { PlayerInventory inv = GetInv(); if (inv != null) { inv.EquipSlot(WeaponSlot.Secondary); inv.RPC_SincronizarSlotRed(WeaponSlot.Secondary); } }
    public void EquipKnifeButton() { PlayerInventory inv = GetInv(); if (inv != null) { inv.EquipSlot(WeaponSlot.Knife); inv.RPC_SincronizarSlotRed(WeaponSlot.Knife); } }
}