using UnityEngine;
using StarterAssets;

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

    private void Update()
    {
        if (starterInputs == null)
            return;

        if (movementJoystick != null)
        {
            Vector2 moveInput = new Vector2(
                movementJoystick.Horizontal,
                movementJoystick.Vertical
            );

            starterInputs.MoveInput(moveInput);
        }

        if (cameraJoystick != null)
        {
            float lookX = cameraJoystick.Horizontal;
            float lookY = cameraJoystick.Vertical;

           
            lookY *= -1f;

            if (invertY)
                lookY *= -1f;

            Vector2 lookInput = new Vector2(lookX, lookY) * cameraSensitivity;

            starterInputs.LookInput(lookInput);
        }
    }

    public void FireButtonDown()
    {
        if (weaponController != null)
            weaponController.MobileFireDown();
    }

    public void FireButtonUp()
    {
        if (weaponController != null)
            weaponController.MobileFireUp();
    }

    public void JumpButtonDown()
    {
        if (starterInputs != null)
            starterInputs.JumpInput(true);
    }

    public void JumpButtonUp()
    {
        if (starterInputs != null)
            starterInputs.JumpInput(false);
    }

    public void EquipPrimaryButton()
    {
        if (playerInventory != null)
            playerInventory.EquipSlot(WeaponSlot.Primary);
    }

    public void EquipSecondaryButton()
    {
        if (playerInventory != null)
            playerInventory.EquipSlot(WeaponSlot.Secondary);
    }

    public void EquipKnifeButton()
    {
        if (playerInventory != null)
            playerInventory.EquipSlot(WeaponSlot.Knife);
    }
}