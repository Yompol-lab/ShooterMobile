using UnityEngine;

public class HUDVictoria : MonoBehaviour
{
    [Header("Carteles de Victoria (UI)")]
    public GameObject cartelGanaPolicia; 
    public GameObject cartelGanaTerro;  

    void Update()
    {
      
        if (MatchManager.Instance == null) return;

       
        if (MatchManager.Instance.EstadoActual == MatchState.RoundEnd || MatchManager.Instance.EstadoActual == MatchState.MatchFinished)
        {
            
            if (MatchManager.Instance.UltimoGanador == Team.Police)
            {
                cartelGanaPolicia.SetActive(true);
                cartelGanaTerro.SetActive(false);
            }
            else if (MatchManager.Instance.UltimoGanador == Team.Terrorist)
            {
                cartelGanaPolicia.SetActive(false);
                cartelGanaTerro.SetActive(true);
            }
        }
        else
        {
            cartelGanaPolicia.SetActive(false);
            cartelGanaTerro.SetActive(false);
        }
    }
}