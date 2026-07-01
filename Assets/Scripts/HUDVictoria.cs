using UnityEngine;

public class HUDVictoria : MonoBehaviour
{
    [Header("Carteles de Victoria (UI)")]
    public GameObject cartelGanaPolicia;
    public GameObject cartelGanaTerro;

    
    private bool musicaYaSonando = false;

    void Update()
    {
        if (MatchManager.Instance == null) return;

        
        if (MatchManager.Instance.EstadoActual == MatchState.RoundEnd || MatchManager.Instance.EstadoActual == MatchState.MatchFinished)
        {
            if (MatchManager.Instance.UltimoGanador == Team.Police)
            {
                cartelGanaPolicia.SetActive(true);
                cartelGanaTerro.SetActive(false);

                if (!musicaYaSonando)
                {
                    AkSoundEngine.PostEvent("Play_CTerrorist_Win", gameObject);
                    musicaYaSonando = true; 
                }
            }
            else if (MatchManager.Instance.UltimoGanador == Team.Terrorist)
            {
                cartelGanaPolicia.SetActive(false);
                cartelGanaTerro.SetActive(true);

                
                if (!musicaYaSonando)
                {
                    AkSoundEngine.PostEvent("Play_Terrorist_Win", gameObject);
                    musicaYaSonando = true; 
                }
            }
        }
        else
        {
           
            cartelGanaPolicia.SetActive(false);
            cartelGanaTerro.SetActive(false);

            
            musicaYaSonando = false;
        }
    }
}