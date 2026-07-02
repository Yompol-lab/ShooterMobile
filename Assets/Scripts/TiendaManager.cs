using UnityEngine;
using TMPro;
using System.Linq;
using Fusion;

public class TiendaManager : MonoBehaviour
{
    [Header("Contenedor Maestro (Fondo y Ruedas)")]
    public GameObject contenedorTienda;

    [Header("Paneles UI Internos")]
    public GameObject panelTiendaPrincipal;
    public GameObject panelRifles;
    public GameObject panelPistolas;
    public GameObject panelHeavy;
    public GameObject panelGranadas;
    public GameObject panelEquipamiento;
    public GameObject panelFaca;
    public GameObject botonAtras;

    [Header("Textos")]
    public TextMeshProUGUI textoDinero;

    private bool estabaEnCompra = false;
    private EconomiaJugador miEconomiaLocal;
    private MatchManager miMatchManager;

    void Update()
    {
        if (miMatchManager == null)
        {
            miMatchManager = MatchManager.Instance;
            if (miMatchManager == null)
                miMatchManager = FindFirstObjectByType<MatchManager>();

            if (miMatchManager == null)
                return;
        }

        if (miMatchManager.Object == null)
            return;

        bool esTiempoCompra = miMatchManager.EstadoActual == MatchState.BuyTime;

        if (!esTiempoCompra)
        {
            if (estabaEnCompra)
            {
                if (contenedorTienda != null)
                    contenedorTienda.SetActive(false);

                estabaEnCompra = false;
            }

            return;
        }

        if (esTiempoCompra && !estabaEnCompra)
        {
            if (contenedorTienda != null)
                contenedorTienda.SetActive(true);

            VolverAtras();
            estabaEnCompra = true;
        }

        if (textoDinero != null)
        {
            if (miEconomiaLocal == null || miEconomiaLocal.Object == null)
            {
                miEconomiaLocal = FindObjectsByType<EconomiaJugador>(FindObjectsSortMode.None)
                    .FirstOrDefault(e => e.Object != null && e.HasInputAuthority);
            }

            if (miEconomiaLocal != null)
                textoDinero.text = "" + miEconomiaLocal.Dinero.ToString();
        }
    }

    private void ApagarRuedasInternas()
    {
        if (panelTiendaPrincipal != null) panelTiendaPrincipal.SetActive(false);
        if (panelRifles != null) panelRifles.SetActive(false);
        if (panelPistolas != null) panelPistolas.SetActive(false);
        if (panelHeavy != null) panelHeavy.SetActive(false);
        if (panelGranadas != null) panelGranadas.SetActive(false);
        if (panelEquipamiento != null) panelEquipamiento.SetActive(false);
        if (panelFaca != null) panelFaca.SetActive(false);

        if (botonAtras != null)
            botonAtras.SetActive(false);
    }

    public void IrARifles()
    {
        ApagarRuedasInternas();
        if (panelRifles != null) panelRifles.SetActive(true);
        if (botonAtras != null) botonAtras.SetActive(true);
    }

    public void IrAPistolas()
    {
        ApagarRuedasInternas();
        if (panelPistolas != null) panelPistolas.SetActive(true);
        if (botonAtras != null) botonAtras.SetActive(true);
    }

    public void IrAHeavy()
    {
        ApagarRuedasInternas();
        if (panelHeavy != null) panelHeavy.SetActive(true);
        if (botonAtras != null) botonAtras.SetActive(true);
    }

    public void IrAGranadas()
    {
        ApagarRuedasInternas();
        if (panelGranadas != null) panelGranadas.SetActive(true);
        if (botonAtras != null) botonAtras.SetActive(true);
    }

    public void IrAEquipamiento()
    {
        ApagarRuedasInternas();
        if (panelEquipamiento != null) panelEquipamiento.SetActive(true);
        if (botonAtras != null) botonAtras.SetActive(true);
    }

    public void IrAFaca()
    {
        ApagarRuedasInternas();
        if (panelFaca != null) panelFaca.SetActive(true);
        if (botonAtras != null) botonAtras.SetActive(true);
    }

    public void VolverAtras()
    {
        ApagarRuedasInternas();

        if (panelTiendaPrincipal != null)
            panelTiendaPrincipal.SetActive(true);
    }

    public void ComprarArma(WeaponData arma)
    {
        if (miMatchManager == null ||
            miMatchManager.Object == null ||
            miMatchManager.EstadoActual != MatchState.BuyTime)
            return;

        if (miEconomiaLocal == null)
            return;

        var miInventario = miEconomiaLocal.GetComponent<PlayerInventory>();

        if (miInventario != null)
        {
            if (miEconomiaLocal.Dinero >= arma.precio)
            {
                if (miInventario.RecibirArmaComprada(arma))
                {
                    miEconomiaLocal.Gastar(arma.precio);
                    Debug.Log($"TIENDA: Compra exitosa de {arma.weaponName}. Cobrado: {arma.precio}");
                }
            }
            else
            {
                Debug.LogWarning("TIENDA: No te alcanza la plata para comprar esta arma.");
            }
        }
    }
}