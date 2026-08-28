using UnityEngine;
using TMPro;
using Fusion;

public class MatchHUD : MonoBehaviour
{
    [Header("Textos de la Interfaz")]
    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoPuntajeT;  
    public TextMeshProUGUI textoPuntajeCT; 
    public TextMeshProUGUI textoNombreSala;

    void Update()
    {
        try
        {
            if (textoNombreSala != null && NetworkManager.Instance != null && NetworkManager.Instance.Runner != null && NetworkManager.Instance.Runner.IsRunning)
            {
                var sessionInfo = NetworkManager.Instance.Runner.SessionInfo;
                if (sessionInfo != null && sessionInfo.IsValid)
                {
                    textoNombreSala.text = "Código de Sala: " + sessionInfo.Name;
                }
            }
        }
        catch (System.Exception) { }

        
        if (MatchManager.Instance != null && MatchManager.Instance.Object != null && MatchManager.Instance.Object.IsValid)
        {
           
            textoPuntajeT.text = MatchManager.Instance.PuntajeTerro.ToString();
            textoPuntajeCT.text = MatchManager.Instance.PuntajePolicia.ToString();

           
            float tiempo = MatchManager.Instance.TiempoRestante;
            if (tiempo < 0) tiempo = 0;

            int minutos = Mathf.FloorToInt(tiempo / 60);
            int segundos = Mathf.FloorToInt(tiempo % 60);

            
            textoTiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }
}