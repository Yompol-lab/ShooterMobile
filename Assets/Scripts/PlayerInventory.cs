using Fusion;
using UnityEngine;
using StarterAssets;

public enum WeaponSlot { Primary, Secondary, Knife, Bomb, Fuego, Humo, Flash, Explosiva }

public class PlayerInventory : NetworkBehaviour
{
    [Header("Configuración de Red")]
    public Transform weaponContainer;
    public Transform dropPoint;
    public float dropForce = 5f;

    [Header("Prefabs de Armas por Defecto")]
    public NetworkPrefabRef prefabPistolaInicio;
    public NetworkPrefabRef prefabCuchilloInicio;

    [Header("C4 y Plantado")]
    public NetworkPrefabRef prefabBombaPlantada;
    public bool enZonaPlantar = false;

    [Header("Slots Actuales (¡Dejar vacíos en el Inspector!)")]
    public GameObject currentPrimary;
    public GameObject currentSecondary;
    public GameObject currentKnife;
    public GameObject currentBomb;

    [Header("Granadas en Mano (¡Dejar vacíos en el Inspector!)")]
    public GameObject currentFuego;
    public GameObject currentHumo;
    public GameObject currentFlash;
    public GameObject currentExplosiva;

    [Networked] public WeaponSlot activeSlot { get; set; }

    private Vector3 ultimaPosicion;

    public override void Spawned()
    {
        currentPrimary = null;
        currentSecondary = null;
        currentKnife = null;
        currentBomb = null;
        currentFuego = null;
        currentHumo = null;
        currentFlash = null;
        currentExplosiva = null;

        ultimaPosicion = transform.position;
        HideAllWeapons();

        if (HasStateAuthority)
        {
            if (prefabCuchilloInicio.IsValid)
            {
                NetworkObject cuchilloNet = Runner.Spawn(prefabCuchilloInicio, dropPoint.position, dropPoint.rotation, Object.InputAuthority);
                RPC_AgarrarArmaRed(cuchilloNet, WeaponSlot.Knife);
            }

            if (prefabPistolaInicio.IsValid)
            {
                NetworkObject pistolaNet = Runner.Spawn(prefabPistolaInicio, dropPoint.position, dropPoint.rotation, Object.InputAuthority);
                RPC_AgarrarArmaRed(pistolaNet, WeaponSlot.Secondary);
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
       
        if (Vector3.Distance(transform.position, ultimaPosicion) > 3f)
        {
            enZonaPlantar = false;
        }
        ultimaPosicion = transform.position;
    }

    private void OnTriggerEnter(Collider other) { if (other.GetComponent<ZonaPlantar>() != null) enZonaPlantar = true; }
    private void OnTriggerExit(Collider other) { if (other.GetComponent<ZonaPlantar>() != null) enZonaPlantar = false; }

    
    public void PlantarBomba()
    {
        if (activeSlot == WeaponSlot.Bomb && currentBomb != null && enZonaPlantar)
        {
            if (MatchManager.Instance != null && MatchManager.Instance.bombaPlantada) return;

            if (HasStateAuthority) EjecutarPlanteRed();
            else RPC_SolicitarPlantar();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SolicitarPlantar()
    {
        if (enZonaPlantar && currentBomb != null) EjecutarPlanteRed();
    }

    private void EjecutarPlanteRed()
    {
        if (MatchManager.Instance != null && MatchManager.Instance.bombaPlantada) return;

        Runner.Spawn(prefabBombaPlantada, transform.position, Quaternion.identity, Runner.LocalPlayer);
        if (MatchManager.Instance != null) MatchManager.Instance.AvisarBombaPlantada(Object.InputAuthority);

        RPC_ConfirmarPlante();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ConfirmarPlante()
    {
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
        if (slot == WeaponSlot.Knife && currentKnife != null) return;

        armaObj.RequestStateAuthority();
        RPC_AgarrarArmaRed(armaObj, slot);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AgarrarArmaRed(NetworkObject armaObj, WeaponSlot slot)
    {
        if (armaObj == null) return;

        if (slot == WeaponSlot.Primary) currentPrimary = armaObj.gameObject;
        else if (slot == WeaponSlot.Secondary) currentSecondary = armaObj.gameObject;
        else if (slot == WeaponSlot.Bomb) currentBomb = armaObj.gameObject;
        else if (slot == WeaponSlot.Knife) currentKnife = armaObj.gameObject;
        else if (slot == WeaponSlot.Fuego) currentFuego = armaObj.gameObject;
        else if (slot == WeaponSlot.Humo) currentHumo = armaObj.gameObject;
        else if (slot == WeaponSlot.Flash) currentFlash = armaObj.gameObject;
        else if (slot == WeaponSlot.Explosiva) currentExplosiva = armaObj.gameObject;

        if (slot == WeaponSlot.Primary || slot == WeaponSlot.Secondary)
        {
            Weapon scriptArma = armaObj.GetComponent<Weapon>();
            if (scriptArma != null && scriptArma.weaponData != null && scriptArma.weaponData.tamañoCargador > 0)
            {
                MunicionArma scriptBala = armaObj.GetComponent<MunicionArma>();
                if (scriptBala == null) scriptBala = armaObj.gameObject.AddComponent<MunicionArma>();
                scriptBala.Configurar(scriptArma.weaponData);
            }
        }

        IngresarArmaAlInventario(armaObj.gameObject);
        EquipSlot(slot);
    }

    private void IngresarArmaAlInventario(GameObject armaObj)
    {
        if (armaObj == null || weaponContainer == null) return;

        NetworkTransform nt = armaObj.GetComponent<NetworkTransform>();
        if (nt != null) nt.enabled = false;

        Rigidbody rb = armaObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider[] colisionadores = armaObj.GetComponentsInChildren<Collider>();
        foreach (Collider c in colisionadores)
        {
            if (!c.isTrigger) c.enabled = false;
        }

        armaObj.transform.SetParent(weaponContainer);

        ArmaEnElPisoRed armaScript = armaObj.GetComponent<ArmaEnElPisoRed>();
        if (armaScript != null)
        {
            armaObj.transform.localPosition = armaScript.holdPosition;
            armaObj.transform.localRotation = Quaternion.Euler(armaScript.holdRotation);
            armaObj.transform.localScale = armaScript.holdScale;
            armaScript.SetFisicas(false);
        }
        else
        {
            armaObj.transform.localPosition = Vector3.zero;
            armaObj.transform.localRotation = Quaternion.identity;
        }

        Animator anim = armaObj.GetComponent<Animator>();
        if (anim != null) anim.enabled = true;
    }

    public void EquipSlot(WeaponSlot slot)
    {
        GetComponent<ControladorMira>()?.CancelarMira();

        PlayerWeaponController weaponController = GetComponent<PlayerWeaponController>();
        if (weaponController != null) weaponController.ResetShooting();

        GameObject equippedWeaponObject = null;
        switch (slot)
        {
            case WeaponSlot.Primary: equippedWeaponObject = currentPrimary; break;
            case WeaponSlot.Secondary: equippedWeaponObject = currentSecondary; break;
            case WeaponSlot.Knife: equippedWeaponObject = currentKnife; break;
            case WeaponSlot.Bomb: equippedWeaponObject = currentBomb; break;
            case WeaponSlot.Fuego: equippedWeaponObject = currentFuego; break;
            case WeaponSlot.Humo: equippedWeaponObject = currentHumo; break;
            case WeaponSlot.Flash: equippedWeaponObject = currentFlash; break;
            case WeaponSlot.Explosiva: equippedWeaponObject = currentExplosiva; break;
        }

        if (equippedWeaponObject == null && slot != WeaponSlot.Knife) return;

        HideAllWeapons();

        if (equippedWeaponObject != null)
        {
            equippedWeaponObject.SetActive(true);
        }

        activeSlot = slot;

        if (weaponController != null)
        {
            Weapon weapon = (equippedWeaponObject != null) ? equippedWeaponObject.GetComponent<Weapon>() : null;
            weaponController.SetCurrentWeapon(weapon);
        }
    }

    public void BotonTirarArma()
    {
        if (Object.HasInputAuthority && (activeSlot == WeaponSlot.Primary || activeSlot == WeaponSlot.Secondary || activeSlot == WeaponSlot.Bomb))
        {
            RPC_TirarArmaRed(activeSlot);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TirarArmaRed(WeaponSlot slotATirar)
    {
        GameObject weaponToDrop = null;
        if (slotATirar == WeaponSlot.Primary) { weaponToDrop = currentPrimary; currentPrimary = null; }
        else if (slotATirar == WeaponSlot.Secondary) { weaponToDrop = currentSecondary; currentSecondary = null; }
        else if (slotATirar == WeaponSlot.Bomb) { weaponToDrop = currentBomb; currentBomb = null; }

        if (weaponToDrop != null)
        {
            weaponToDrop.SetActive(true);
            Animator anim = weaponToDrop.GetComponent<Animator>();
            if (anim != null) anim.enabled = false;

            weaponToDrop.transform.SetParent(null);

            NetworkTransform nt = weaponToDrop.GetComponent<NetworkTransform>();
            if (nt != null)
            {
                nt.enabled = true;
                weaponToDrop.transform.position = dropPoint.position;
                weaponToDrop.transform.rotation = dropPoint.rotation;
                if (HasStateAuthority) nt.Teleport(dropPoint.position, dropPoint.rotation);
            }

            Rigidbody rb = weaponToDrop.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                if (HasStateAuthority)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.AddForce(dropPoint.forward * dropForce, ForceMode.Impulse);
                }
            }

            Collider[] colisionadores = weaponToDrop.GetComponentsInChildren<Collider>();
            foreach (Collider c in colisionadores)
            {
                if (!c.isTrigger) c.enabled = true;
            }

            ArmaEnElPisoRed armaScript = weaponToDrop.GetComponent<ArmaEnElPisoRed>();
            if (armaScript != null) armaScript.SetFisicas(true);
        }

        if (currentKnife != null) EquipSlot(WeaponSlot.Knife);
    }

    private void HideAllWeapons()
    {
        if (currentPrimary != null) currentPrimary.SetActive(false);
        if (currentSecondary != null) currentSecondary.SetActive(false);
        if (currentKnife != null) currentKnife.SetActive(false);
        if (currentBomb != null) currentBomb.SetActive(false);
        if (currentFuego != null) currentFuego.SetActive(false);
        if (currentHumo != null) currentHumo.SetActive(false);
        if (currentFlash != null) currentFlash.SetActive(false);
        if (currentExplosiva != null) currentExplosiva.SetActive(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SincronizarSlotRed(WeaponSlot nuevoSlot)
    {
        if (!HasStateAuthority) EquipSlot(nuevoSlot);
    }

    public bool RecibirArmaComprada(WeaponData datosArma)
    {
        if (datosArma == null) return false;

        if (datosArma.weaponSlot == WeaponSlot.Fuego || datosArma.weaponSlot == WeaponSlot.Humo ||
            datosArma.weaponSlot == WeaponSlot.Flash || datosArma.weaponSlot == WeaponSlot.Explosiva)
        {
            if (HasStateAuthority)
            {
                ControladorGranadasRed controlGranadas = GetComponent<ControladorGranadasRed>();
                if (controlGranadas != null)
                {
                    if (datosArma.weaponSlot == WeaponSlot.Fuego) controlGranadas.granadasFuego++;
                    else if (datosArma.weaponSlot == WeaponSlot.Humo) controlGranadas.granadasHumo++;
                    else if (datosArma.weaponSlot == WeaponSlot.Flash) controlGranadas.granadasFlash++;
                    else if (datosArma.weaponSlot == WeaponSlot.Explosiva) controlGranadas.granadasExplosivas++;
                }

                bool necesitaModelo = false;
                if (datosArma.weaponSlot == WeaponSlot.Fuego && currentFuego == null) necesitaModelo = true;
                else if (datosArma.weaponSlot == WeaponSlot.Humo && currentHumo == null) necesitaModelo = true;
                else if (datosArma.weaponSlot == WeaponSlot.Flash && currentFlash == null) necesitaModelo = true;
                else if (datosArma.weaponSlot == WeaponSlot.Explosiva && currentExplosiva == null) necesitaModelo = true;

                if (necesitaModelo && datosArma.prefabParaMano != null)
                {
                    NetworkObject nuevaArmaNet = Runner.Spawn(datosArma.prefabParaMano.GetComponent<NetworkObject>(), dropPoint.position, dropPoint.rotation, Object.InputAuthority);
                    RPC_AgarrarArmaRed(nuevaArmaNet, datosArma.weaponSlot);
                }
            }
            return true;
        }

        if (datosArma.prefabParaMano == null || weaponContainer == null) return false;
        NetworkObject prefabNet = datosArma.prefabParaMano.GetComponent<NetworkObject>();

        if (prefabNet != null && HasStateAuthority)
        {
            if (datosArma.weaponSlot == WeaponSlot.Primary && currentPrimary != null) { RPC_TirarArmaRed(WeaponSlot.Primary); currentPrimary = null; }
            else if (datosArma.weaponSlot == WeaponSlot.Secondary && currentSecondary != null) { RPC_TirarArmaRed(WeaponSlot.Secondary); currentSecondary = null; }
            else if (datosArma.weaponSlot == WeaponSlot.Knife && currentKnife != null)
            {
                NetworkObject c = currentKnife.GetComponent<NetworkObject>();
                if (c != null) Runner.Despawn(c); else Destroy(currentKnife);
                currentKnife = null;
            }

            NetworkObject nuevaArmaNet = Runner.Spawn(prefabNet, dropPoint.position, dropPoint.rotation, Object.InputAuthority);
            RPC_AgarrarArmaRed(nuevaArmaNet, datosArma.weaponSlot);
            return true;
        }

        return false;
    }

    public GameObject GetActiveWeaponObject()
    {
        if (activeSlot == WeaponSlot.Primary) return currentPrimary;
        if (activeSlot == WeaponSlot.Secondary) return currentSecondary;
        if (activeSlot == WeaponSlot.Knife) return currentKnife;
        if (activeSlot == WeaponSlot.Bomb) return currentBomb;
        if (activeSlot == WeaponSlot.Fuego) return currentFuego;
        if (activeSlot == WeaponSlot.Humo) return currentHumo;
        if (activeSlot == WeaponSlot.Flash) return currentFlash;
        if (activeSlot == WeaponSlot.Explosiva) return currentExplosiva;
        return null;
    }

    public void ConsumirGranadaMano(WeaponSlot slot)
    {
        GameObject granadaObj = null;
        if (slot == WeaponSlot.Fuego) { granadaObj = currentFuego; currentFuego = null; }
        else if (slot == WeaponSlot.Humo) { granadaObj = currentHumo; currentHumo = null; }
        else if (slot == WeaponSlot.Flash) { granadaObj = currentFlash; currentFlash = null; }
        else if (slot == WeaponSlot.Explosiva) { granadaObj = currentExplosiva; currentExplosiva = null; }

        if (granadaObj != null)
        {
            NetworkObject no = granadaObj.GetComponent<NetworkObject>();
            if (no != null && HasStateAuthority) Runner.Despawn(no);
            else Destroy(granadaObj);
        }
    }

   
    public void BotonBomba_MantenerPresionado()
    {
        ConfiguracionJugadorRed config = GetComponent<ConfiguracionJugadorRed>();

        if (config != null && config.miEquipo == Team.Terrorist)
        {
            if (activeSlot != WeaponSlot.Bomb)
            {
                if (currentBomb != null) EquipSlot(WeaponSlot.Bomb);
            }
            else
            {
                PlantarBomba();
            }
        }
        else if (config != null && config.miEquipo == Team.Police)
        {
            if (LogicaBombaPlantada.BombaActiva != null)
            {
                float distancia = Vector3.Distance(transform.position, LogicaBombaPlantada.BombaActiva.transform.position);
                if (distancia <= 3f)
                {
                    LogicaBombaPlantada.BombaActiva.RPC_IntentarDefusar(Object.InputAuthority, true);
                }
            }
        }
    }

    public void BotonBomba_SoltarBoton()
    {
        ConfiguracionJugadorRed config = GetComponent<ConfiguracionJugadorRed>();

        if (config != null && config.miEquipo == Team.Police)
        {
            if (LogicaBombaPlantada.BombaActiva != null)
            {
                LogicaBombaPlantada.BombaActiva.RPC_IntentarDefusar(Object.InputAuthority, false);
            }
        }
    }
}