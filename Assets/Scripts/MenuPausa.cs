using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using System.Collections; 

public class MenuPausa : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Arrastrá acá el Panel hijo que contiene los 3 botones")]
    public GameObject panelPausa;

    [Header("Configuración")]
    [Tooltip("El nombre exacto de la escena de tu menú principal")]
    public string nombreEscenaMenu = "Menu";

    private void Start()
    {
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }
    }

    public void AbrirPausa()
    {
        if (panelPausa != null)
        {
            panelPausa.SetActive(true);
        }
    }

    public void ReanudarJuego()
    {
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }
    }

    public void VolverAlMenu()
    {
        
        StartCoroutine(RutinaDesconectar());
    }

    private IEnumerator RutinaDesconectar()
    {
        if (panelPausa != null) panelPausa.SetActive(false);

        NetworkRunner runner = FindFirstObjectByType<NetworkRunner>();
        if (runner != null)
        {
            runner.Shutdown();
            yield return new WaitForSecondsRealtime(0.5f); 

            
            if (runner != null) Destroy(runner.gameObject);
        }

        MatchManager viejoManager = FindFirstObjectByType<MatchManager>();
        if (viejoManager != null) Destroy(viejoManager.gameObject);

        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
}