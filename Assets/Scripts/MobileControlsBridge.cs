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

    
    public void BotonRecargarUI()
    {
        if (playerInventory != null)
        {
            GameObject armaObj = playerInventory.GetActiveWeaponObject();
            if (armaObj != null)
            {
                MunicionArma mun = armaObj.GetComponent<MunicionArma>();
                if (mun != null) mun.IniciarRecarga();
            }
        }
    }
  

    public void BotonTirarArma()
    {
        if (playerInventory != null) playerInventory.BotonTirarArma();
    }

    public void BotonSacarBomba()
    {
        if (playerInventory != null)
        {
            if (playerInventory.activeSlot == WeaponSlot.Bomb && playerInventory.enZonaPlantar)
            {
                playerInventory.PlantarBomba();
            }
            else if (playerInventory.activeSlot == WeaponSlot.Bomb)
            {
                playerInventory.EquipSlot(WeaponSlot.Knife);
                playerInventory.RPC_SincronizarSlotRed(WeaponSlot.Knife);
            }
            else
            {
                playerInventory.EquipSlot(WeaponSlot.Bomb);
                playerInventory.RPC_SincronizarSlotRed(WeaponSlot.Bomb);
            }
        }
    }

    private void Update()
    {
        
        if (textoMunicionHUD != null && playerInventory != null)
        {
            GameObject armaObj = playerInventory.GetActiveWeaponObject();
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

    public void EquipPrimaryButton() { if (playerInventory != null) { playerInventory.EquipSlot(WeaponSlot.Primary); playerInventory.RPC_SincronizarSlotRed(WeaponSlot.Primary); } }
    public void EquipSecondaryButton() { if (playerInventory != null) { playerInventory.EquipSlot(WeaponSlot.Secondary); playerInventory.RPC_SincronizarSlotRed(WeaponSlot.Secondary); } }
    public void EquipKnifeButton() { if (playerInventory != null) { playerInventory.EquipSlot(WeaponSlot.Knife); playerInventory.RPC_SincronizarSlotRed(WeaponSlot.Knife); } }
}