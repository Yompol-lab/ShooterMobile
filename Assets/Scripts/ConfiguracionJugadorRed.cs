using Fusion;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

public class ConfiguracionJugadorRed : NetworkBehaviour
{
    [Header("Componentes a apagar en los RIVALES")]
    public Camera camaraDelJugador;
    public AudioListener audioListener;
    public PlayerInput playerInput;
    public FirstPersonController controladorMovimiento;

    [Header("Scripts para conectar a MIS controles")]
    public StarterAssetsInputs misInputs;
    public PlayerWeaponController miArma;
    public PlayerInventory miInventario;

    public override void Spawned()
    {

        if (HasStateAuthority)
        {
            MobileControlsBridge mobileControls = FindFirstObjectByType<MobileControlsBridge>();

            if (mobileControls != null)
            {
                mobileControls.starterInputs = misInputs;
                mobileControls.weaponController = miArma;
                mobileControls.playerInventory = miInventario;
            }
        }
        else
        {
            
            if (camaraDelJugador != null) camaraDelJugador.gameObject.SetActive(false);
            if (audioListener != null) audioListener.enabled = false;
            if (playerInput != null) playerInput.enabled = false;
            if (controladorMovimiento != null) controladorMovimiento.enabled = false;
        }
    }
}