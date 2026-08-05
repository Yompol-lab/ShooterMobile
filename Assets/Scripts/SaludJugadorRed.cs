using Fusion;
using UnityEngine;
using StarterAssets;
using System.Collections;

public class SaludJugadorRed : NetworkBehaviour
{
    [Header("Configuración")]
    [Networked] public int Vida { get; set; } = 100;

    [Header("UI")]
    [SerializeField] private GameObject panelHUD;

    private EfectoRagdollRed ragdoll;
    private bool estaMuerto = false;

    public override void Spawned()
    {
        ragdoll = GetComponent<EfectoRagdollRed>();
        estaMuerto = false;
        Vida = 100;

        if (HasStateAuthority)
        {
            HUDPrincipal miHud = FindFirstObjectByType<HUDPrincipal>();

            if (miHud != null)
            {
                panelHUD = miHud.gameObject;
            }
            
        }
    }

    public void RestaurarVidaAlMaximo()
    {
        if (HasStateAuthority)
        {
            Vida = 100;
            estaMuerto = false;
            RPC_RevivirRed();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TomarDanio(int cantidad, Vector3 posicionDelOrígen)
    {
        if (estaMuerto) return;

        Vida -= cantidad;

        if (Vida <= 0)
        {
            Vida = 0;
            estaMuerto = true;

            Vector3 direccionEmpujon = (transform.position - posicionDelOrígen).normalized;
            direccionEmpujon += Vector3.up * 0.5f;

            if (ragdoll != null)
            {
                ragdoll.Morir(direccionEmpujon);
            }

            StartCoroutine(RutinaRespawn());

          
            if (MatchManager.Instance != null) MatchManager.Instance.VerificarBajas();
        }
    }

    private IEnumerator RutinaRespawn()
    {
        ConfiguracionJugadorRed miConfig = GetComponent<ConfiguracionJugadorRed>();
        PlayerInventory miInventario = GetComponent<PlayerInventory>();

        if (HasStateAuthority)
        {
            if (miInventario != null)
            {
                if (miInventario.currentPrimary != null) miInventario.RPC_TirarArmaRed(WeaponSlot.Primary);
                if (miInventario.currentBomb != null) miInventario.RPC_TirarArmaRed(WeaponSlot.Bomb);

                if (miInventario.currentSecondary != null)
                {
                    miInventario.EquipSlot(WeaponSlot.Secondary);
                    miInventario.RPC_SincronizarSlotRed(WeaponSlot.Secondary);
                }
                else
                {
                    miInventario.EquipSlot(WeaponSlot.Knife);
                    miInventario.RPC_SincronizarSlotRed(WeaponSlot.Knife);
                }
            }

            if (miConfig != null)
            {
                Camera camaraPrincipal = Camera.main;
                if (camaraPrincipal != null) camaraPrincipal.gameObject.SetActive(true);
                if (miConfig.camaraDelJugador != null) miConfig.camaraDelJugador.gameObject.SetActive(false);
                if (panelHUD != null) panelHUD.SetActive(false);

                if (miConfig.misInputs != null)
                {
                    miConfig.misInputs.move = Vector2.zero;
                    miConfig.misInputs.look = Vector2.zero;
                    miConfig.misInputs.jump = false;
                }
            }
        }

        if (MatchManager.Instance != null)
        {
            yield return new WaitUntil(() => MatchManager.Instance.EstadoActual == MatchState.BuyTime);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(5f);
        }

        Vector3 posicionBase = new Vector3(0, 10, 0);
        Quaternion rotacionBase = Quaternion.identity;

        if (miConfig != null)
        {
            TeamSpawnPoint[] todosLosSpawns = FindObjectsByType<TeamSpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            var spawnsValidos = System.Array.FindAll(todosLosSpawns, sp => sp.team == miConfig.miEquipo);

            if (spawnsValidos.Length > 0)
            {
                int rand = Random.Range(0, spawnsValidos.Length);
                posicionBase = spawnsValidos[rand].transform.position + (Vector3.up * 2.0f);
                rotacionBase = spawnsValidos[rand].transform.rotation;
            }
        }

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.position = posicionBase;
        transform.rotation = rotacionBase;

        NetworkTransform netTransform = GetComponent<NetworkTransform>();
        if (netTransform != null) netTransform.Teleport(posicionBase);

        yield return new WaitForFixedUpdate();
        yield return null;

        Vida = 100;
        estaMuerto = false;

        if (cc != null) cc.enabled = true;

        RPC_RevivirRed();

        if (HasStateAuthority && miConfig != null)
        {
            Camera camaraPrincipal = Camera.main;
            if (camaraPrincipal != null) camaraPrincipal.gameObject.SetActive(false);
            if (miConfig.camaraDelJugador != null) miConfig.camaraDelJugador.gameObject.SetActive(true);
            if (panelHUD != null) panelHUD.SetActive(true);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_RevivirRed()
    {
        if (ragdoll != null)
        {
            ragdoll.Revivir();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RPC_CegarPantallaLocal()
    {
        MobileControlsBridge uiMobile = FindFirstObjectByType<MobileControlsBridge>();
        if (uiMobile != null)
        {
            uiMobile.StartCoroutine(uiMobile.RutinaEfectoFlash());
        }
    }
}