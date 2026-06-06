using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject panelMenu;
    public GameObject panelJugar;
    public GameObject panelPerfil;
    public GameObject panelOpciones;
    public GameObject panelGrupo;

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

    public void AbrirGrupo()
    {
        panelJugar.SetActive(false);
        panelGrupo.SetActive(true);
    }

    public void VolverAJugar()
    {
        panelGrupo.SetActive(false);
        panelJugar.SetActive(true);
    }

    public void VolverMenu()
    {
        panelMenu.SetActive(true);
        panelJugar.SetActive(false);
        panelPerfil.SetActive(false);
        panelOpciones.SetActive(false);
        panelGrupo.SetActive(false);
    }

    public void BuscarPartida()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
}