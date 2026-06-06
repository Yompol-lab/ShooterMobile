using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject panelMenu;
    public GameObject panelJugar;
    public GameObject panelPerfil;
    public GameObject panelOpciones;

    public void AbrirJugar()
    {
        panelMenu.SetActive(false);
        panelJugar.SetActive(true);
    }

    public void AbrirPerfil()
    {
        panelMenu.SetActive(false);
        panelPerfil.SetActive(true);
    }

    public void AbrirOpciones()
    {
        panelMenu.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void VolverMenu()
    {
        panelMenu.SetActive(true);
        panelJugar.SetActive(false);
        panelPerfil.SetActive(false);
        panelOpciones.SetActive(false);
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
}
