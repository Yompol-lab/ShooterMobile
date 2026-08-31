using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject panelMenu;
    public GameObject panelJugar;
    public GameObject panelPerfil;
    public GameObject panelOpciones;
    public GameObject panelGrupo;
    public GameObject panelCreditos;

    [Header("Networking")]
    public TMP_InputField inputNombreSala;

    [Header("Configuración de Créditos")]
    public RectTransform textoCreditos;
    public float velocidadScroll = 100f;
    public float posicionInicialY = -800f;

    [Header("Configuración de Audio")]
    [Tooltip("Arrastrá acá el objeto que tiene el AkAmbient que reproduce la música del menú (ej: Main Camera u objeto vacío)")]
    public GameObject objetoEmisorDeAudio;

    public string playCreditos = "Play_MusicaCreditos";
    public string stopCreditos = "StopMusicaCreditos";
    public string pauseMenu = "Pause_MenuMusic";
    public string resumeMenu = "Resume_MenuMusic";

    private bool creditosActivos = false;

    void Update()
    {
        if (creditosActivos && textoCreditos != null)
        {
            textoCreditos.anchoredPosition += Vector2.up * velocidadScroll * Time.deltaTime;
        }
    }

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

    public void AbrirCreditos()
    {
        panelMenu.SetActive(false);
        panelJugar.SetActive(false);
        panelPerfil.SetActive(false);
        panelOpciones.SetActive(false);
        panelGrupo.SetActive(false);

        if (panelCreditos != null) panelCreditos.SetActive(true);

        if (textoCreditos != null)
        {
            textoCreditos.anchoredPosition = new Vector2(textoCreditos.anchoredPosition.x, posicionInicialY);
        }

        creditosActivos = true;

       
        if (objetoEmisorDeAudio != null)
        {
            AkSoundEngine.PostEvent(pauseMenu, objetoEmisorDeAudio);
            AkSoundEngine.PostEvent(playCreditos, objetoEmisorDeAudio);
        }
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

        if (panelCreditos != null) panelCreditos.SetActive(false);
        creditosActivos = false;

        
        if (objetoEmisorDeAudio != null)
        {
            AkSoundEngine.PostEvent(stopCreditos, objetoEmisorDeAudio);
            AkSoundEngine.PostEvent(resumeMenu, objetoEmisorDeAudio);
        }
    }

    public void BuscarPartida()
    {
        if (inputNombreSala != null && !string.IsNullOrEmpty(inputNombreSala.text))
        {
            // Join specific room
            string nombreSala = inputNombreSala.text.ToUpper();
            PlayerPrefs.SetString("RoomName", nombreSala);
            PlayerPrefs.Save();
            
            if (GroupManager.EnGrupo && GroupManager.SoyLider)
            {
                GroupManager.Instance.EnviarComandoIrAPartida(nombreSala);
            }
            else
            {
                SceneManager.LoadScene("SampleScene");
            }
        }
        else
        {
            // Run Matchmaking
            if (GroupManager.EnGrupo && !GroupManager.SoyLider)
            {
                Debug.Log("Solo el líder puede buscar partida.");
                return;
            }

            Debug.Log("Iniciando búsqueda de partida...");
            
            MatchmakingScanner scanner = gameObject.AddComponent<MatchmakingScanner>();
            int partySize = GroupManager.EnGrupo ? 2 : 1; // Default to 2 for group, but could be dynamic
            
            scanner.FindMatch(partySize, (roomCode) => 
            {
                if (GroupManager.EnGrupo && GroupManager.SoyLider)
                {
                    GroupManager.Instance.EnviarComandoIrAPartida(roomCode);
                }
                else
                {
                    PlayerPrefs.SetString("RoomName", roomCode);
                    PlayerPrefs.Save();
                    SceneManager.LoadScene("SampleScene");
                }
            });
        }
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
}
