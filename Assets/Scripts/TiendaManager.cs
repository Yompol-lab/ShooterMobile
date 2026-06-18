using UnityEngine;
using TMPro;
using System.Linq;

public class TiendaManager : MonoBehaviour
{
    [Header("Paneles UI")]
    public GameObject panelTiendaPrincipal;
    public GameObject panelRifles;
    public GameObject panelPistolas;

    [Header("Textos")]
    public TextMeshProUGUI textoDinero;

    private bool estabaEnCompra = false;

    void Update()
    {
        if (MatchManager.Instance == null) return;

        bool esTiempoCompra = MatchManager.Instance.EstadoActual == MatchState.BuyTime;

        if (!esTiempoCompra)
        {
            if (estabaEnCompra)
            {
                panelTiendaPrincipal.SetActive(false);
                panelRifles.SetActive(false);
                panelPistolas.SetActive(false);
                estabaEnCompra = false;
            }
            return;
        }

        if (esTiempoCompra && !estabaEnCompra)
        {
            panelTiendaPrincipal.SetActive(true);
            panelRifles.SetActive(false);
            panelPistolas.SetActive(false);
            estabaEnCompra = true;
        }

        var miEco = FindObjectsByType<EconomiaJugador>(FindObjectsSortMode.None).FirstOrDefault(e => e.HasInputAuthority);
        if (miEco != null && textoDinero != null)
        {
            textoDinero.text = "$ " + miEco.Dinero.ToString();
        }
    }

    public void IrARifles() { panelTiendaPrincipal.SetActive(false); panelRifles.SetActive(true); }
    public void IrAPistolas() { panelTiendaPrincipal.SetActive(false); panelPistolas.SetActive(true); }
    public void VolverAtras() { panelRifles.SetActive(false); panelPistolas.SetActive(false); panelTiendaPrincipal.SetActive(true); }

    
    public void ComprarArma(WeaponData arma)
    {
        var miEco = FindObjectsByType<EconomiaJugador>(FindObjectsSortMode.None).FirstOrDefault(e => e.HasInputAuthority);
        var miInventario = FindObjectsByType<PlayerInventory>(FindObjectsSortMode.None).FirstOrDefault(i => i.HasInputAuthority);

        if (miEco != null && miInventario != null)
        {
            if (miEco.Gastar(arma.precio))
            {
                miInventario.RecibirArmaComprada(arma);
            }
        }
    }
}