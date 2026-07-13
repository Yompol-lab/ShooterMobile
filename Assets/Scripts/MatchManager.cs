using Fusion;
using UnityEngine;
using StarterAssets; 

public enum MotivoFinRonda { Eliminacion, Tiempo, Desactivacion, Detonacion }
public enum TipoEquipo { AntiTerrorista, Terrorista, Ninguno }


public enum MatchState { Warmup, BuyTime, InProgress, BombPlanted, RoundEnd, MatchFinished }

public class MatchManager : NetworkBehaviour
{
    public static MatchManager Instance { get; private set; }

    [Header("Estados de Partida (Restaurados)")]
    [Networked] public MatchState EstadoActual { get; set; }

   
    [Networked] public float TiempoRestante { get; set; }
    [Networked] public int PuntajeTerro { get; set; }
    [Networked] public int PuntajePolicia { get; set; }
    [Networked] public Team UltimoGanador { get; set; }

    [Header("Estado Sincronizado de Bomba")]
    [Networked] public bool bombaPlantada { get; set; } = false;
    [Networked] public PlayerRef jugadorQuePlanto { get; set; } = default;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    
    public void AvisarBombaPlantada(PlayerRef planter = default)
    {
        if (!HasStateAuthority) return;
        bombaPlantada = true;
        jugadorQuePlanto = planter;
        EstadoActual = MatchState.BombPlanted; 
        Debug.Log($" MATCH: Bomba plantada");
    }

   
    public void FinalizarRonda(TipoEquipo equipoGanador, MotivoFinRonda motivo, PlayerRef jugadorEspecial = default)
    {
        if (!HasStateAuthority) return;

       
        EconomiaJugador[] todasLasEconomias = FindObjectsByType<EconomiaJugador>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (EconomiaJugador eco in todasLasEconomias)
        {
            PlayerRef jugadorRef = eco.Object.InputAuthority;
            ConfiguracionJugadorRed config = eco.GetComponent<ConfiguracionJugadorRed>();

            if (config == null) continue;

            TipoEquipo bandoJugador = TipoEquipo.Ninguno;
            string nombreTeam = config.miEquipo.ToString().ToLower();

            if (nombreTeam.Contains("anti") || nombreTeam.Contains("counter") || nombreTeam == "ct" || nombreTeam.Contains("policia"))
            {
                bandoJugador = TipoEquipo.AntiTerrorista;
            }
            else if (nombreTeam.Contains("terror") || nombreTeam == "t")
            {
                bandoJugador = TipoEquipo.Terrorista;
            }

            int plataOtorgada = 0;

            
            if (equipoGanador == TipoEquipo.AntiTerrorista)
            {
                if (bandoJugador == TipoEquipo.AntiTerrorista)
                {
                    if (motivo == MotivoFinRonda.Eliminacion || motivo == MotivoFinRonda.Tiempo) plataOtorgada = 3250;
                    else if (motivo == MotivoFinRonda.Desactivacion)
                    {
                        plataOtorgada = 3500;
                        if (jugadorRef == jugadorEspecial) plataOtorgada += 300;
                    }
                }
                else if (bandoJugador == TipoEquipo.Terrorista)
                {
                    if (!bombaPlantada) plataOtorgada = 0;
                    else plataOtorgada = 800;
                }
            }
           
            else if (equipoGanador == TipoEquipo.Terrorista)
            {
                if (bandoJugador == TipoEquipo.Terrorista)
                {
                    if (motivo == MotivoFinRonda.Eliminacion) plataOtorgada = 3250;
                    else if (motivo == MotivoFinRonda.Detonacion)
                    {
                        plataOtorgada = 3500;
                        if (jugadorRef == jugadorQuePlanto) plataOtorgada += 300;
                    }
                }
                else if (bandoJugador == TipoEquipo.AntiTerrorista)
                {
                    plataOtorgada = 1400;
                }
            }

            if (plataOtorgada > 0)
            {
                eco.RPC_SincronizarPremioRonda(plataOtorgada);
            }
        }

        bombaPlantada = false;
        jugadorQuePlanto = default;
    }
}