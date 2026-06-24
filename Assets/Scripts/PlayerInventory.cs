using Fusion;
using UnityEngine;
using StarterAssets;
public enum WeaponSlot { Primary, Secondary, Knife, Bomb, Utility }

public class PlayerInventory : NetworkBehaviour
{
    [Header("Configuración de Red")]
    public Transform weaponContainer;
    public Transform dropPoint;
    public float dropForce = 5f;

    [Header("C4 y Plantado")]
    public NetworkPrefabRef prefabBombaPlantada;
    public bool enZonaPlantar = false;

    [Header("Slots Actuales")]
    public GameObject currentPrimary;
    public GameObject currentSecondary;
    public GameObject currentKnife;
    public GameObject currentBomb;

    [Networked] public WeaponSlot activeSlot { get; set; }

    public override void Spawned()
    {
        if (currentPrimary != null) currentPrimary.SetActive(false);
        if (currentBomb != null) currentBomb.SetActive(false);

        if (currentSecondary != null) EquipSlot(WeaponSlot.Secondary);
        else if (currentKnife != null) EquipSlot(WeaponSlot.Knife);
    }

    private void OnTriggerEnter(Collider other) { if (other.GetComponent<ZonaPlantar>() != null) enZonaPlantar = true; }
    private void OnTriggerExit(Collider other) { if (other.GetComponent<ZonaPlantar>() != null) enZonaPlantar = false; }

    public void PlantarBomba()
    {
        if (HasStateAuthority && activeSlot == WeaponSlot.Bomb && enZonaPlantar)
        {
            RPC_PlantarBombaRed();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlantarBombaRed()
    {
        if (HasStateAuthority)
        {
            Runner.Spawn(prefabBombaPlantada, transform.position, Quaternion.identity, Runner.LocalPlayer);
            if (MatchManager.Instance != null) MatchManager.Instance.AvisarBombaPlantada();
        }

        if (currentBomb != null)
        {
            NetworkObject bombaNet = currentBomb.GetComponent<NetworkObject>();
            if (bombaNet != null && HasStateAuthority) Runner.Despawn(bombaNet);
            currentBomb = null;
        }

        EquipSlot(WeaponSlot.Knife);
    }

    public void JuntarArmaDelPiso(NetworkObject armaObj, WeaponSlot slot)
    {
        if (slot == WeaponSlot.Primary && currentPrimary != null) return;
        if (slot == WeaponSlot.Secondary && currentSecondary != null) return;
        if (slot == WeaponSlot.Bomb && currentBomb != null) return;

        armaObj.RequestStateAuthority();
        RPC_AgarrarArmaRed(armaObj, slot);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AgarrarArmaRed(NetworkObject armaObj, WeaponSlot slot)
    {
        if (armaObj == null) return;

        armaObj.transform.SetParent(weaponContainer);
        ArmaEnElPisoRed armaScript = armaObj.GetComponent<ArmaEnElPisoRed>();
        if (armaScript != null)
        {
            armaObj.transform.localPosition = armaScript.holdPosition;
            armaObj.transform.localRotation = Quaternion.Euler(armaScript.holdRotation);
            armaObj.transform.localScale = armaScript.holdScale;
            armaScript.SetFisicas(false);
        }

        if (slot == WeaponSlot.Primary) currentPrimary = armaObj.gameObject;
        else if (slot == WeaponSlot.Secondary) currentSecondary = armaObj.gameObject;
        else if (slot == WeaponSlot.Bomb) currentBomb = armaObj.gameObject;

        EquipSlot(slot);
    }

    public void BotonTirarArma()
    {
        if (HasStateAuthority && activeSlot != WeaponSlot.Knife) RPC_TirarArmaRed(activeSlot);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_TirarArmaRed(WeaponSlot slotATirar)
    {
        GameObject weaponToDrop = null;
        if (slotATirar == WeaponSlot.Primary) { weaponToDrop = currentPrimary; currentPrimary = null; }
        else if (slotATirar == WeaponSlot.Secondary) { weaponToDrop = currentSecondary; currentSecondary = null; }
        else if (slotATirar == WeaponSlot.Bomb) { weaponToDrop = currentBomb; currentBomb = null; }

        if (weaponToDrop != null)
        {
            weaponToDrop.transform.SetParent(null);
            weaponToDrop.transform.position = dropPoint.position;
            weaponToDrop.transform.rotation = dropPoint.rotation;

            ArmaEnElPisoRed armaScript = weaponToDrop.GetComponent<ArmaEnElPisoRed>();
            if (armaScript != null) armaScript.SetFisicas(true);

            if (HasStateAuthority)
            {
                Rigidbody rb = weaponToDrop.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.AddForce(dropPoint.forward * dropForce, ForceMode.Impulse);
                }
            }
        }
        if (currentKnife != null) EquipSlot(WeaponSlot.Knife);
    }

    public void EquipSlot(WeaponSlot slot)
    {
        GameObject equippedWeaponObject = null;
        switch (slot)
        {
            case WeaponSlot.Primary: equippedWeaponObject = currentPrimary; break;
            case WeaponSlot.Secondary: equippedWeaponObject = currentSecondary; break;
            case WeaponSlot.Knife: equippedWeaponObject = currentKnife; break;
            case WeaponSlot.Bomb: equippedWeaponObject = currentBomb; break;
        }

        if (equippedWeaponObject == null && slot != WeaponSlot.Knife) return;

        HideAllWeapons();
        if (equippedWeaponObject != null) equippedWeaponObject.SetActive(true);
        activeSlot = slot;

        PlayerWeaponController weaponController = GetComponent<PlayerWeaponController>();
        if (weaponController != null)
        {
            Weapon weapon = (equippedWeaponObject != null) ? equippedWeaponObject.GetComponent<Weapon>() : null;
            if (weapon != null) weaponController.SetCurrentWeapon(weapon);
        }
    }

    private void HideAllWeapons()
    {
        if (currentPrimary != null) currentPrimary.SetActive(false);
        if (currentSecondary != null) currentSecondary.SetActive(false);
        if (currentKnife != null) currentKnife.SetActive(false);
        if (currentBomb != null) currentBomb.SetActive(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SincronizarSlotRed(WeaponSlot nuevoSlot)
    {
        if (!HasStateAuthority) EquipSlot(nuevoSlot);
    }

    public bool RecibirArmaComprada(WeaponData datosArma)
    {
        if (datosArma.prefabParaMano == null)
        {
            Debug.LogError($" ERROR: El archivo WeaponData '{datosArma.weaponName}' no tiene puesto el 'Prefab Para Mano' en el Inspector.");
            return false;
        }
        if (weaponContainer == null)
        {
            Debug.LogError(" ERROR: El script PlayerInventory de tu jugador no tiene asignado el 'Weapon Container' (la cámara) en el Inspector.");
            return false;
        }

        GameObject nuevaArma = Instantiate(datosArma.prefabParaMano, weaponContainer);

        nuevaArma.transform.localPosition = Vector3.zero;
        nuevaArma.transform.localRotation = Quaternion.identity;
        nuevaArma.transform.localScale = Vector3.one;

        
        if (datosArma.tamañoCargador > 0)
        {
            MunicionArma scriptBala = nuevaArma.AddComponent<MunicionArma>();
            scriptBala.Configurar(datosArma);
        }
       

        if (datosArma.weaponSlot == WeaponSlot.Primary)
        {
            if (currentPrimary != null) Destroy(currentPrimary);
            currentPrimary = nuevaArma;
        }
        else if (datosArma.weaponSlot == WeaponSlot.Secondary)
        {
            if (currentSecondary != null) Destroy(currentSecondary);
            currentSecondary = nuevaArma;
        }

        EquipSlot(datosArma.weaponSlot);
        if (Object.HasStateAuthority) RPC_SincronizarSlotRed(datosArma.weaponSlot);

        return true;
    }

   
    public GameObject GetActiveWeaponObject()
    {
        if (activeSlot == WeaponSlot.Primary) return currentPrimary;
        if (activeSlot == WeaponSlot.Secondary) return currentSecondary;
        if (activeSlot == WeaponSlot.Knife) return currentKnife;
        if (activeSlot == WeaponSlot.Bomb) return currentBomb;
        return null;
    }
}