using Fusion;
using UnityEngine;
using StarterAssets;

public enum MotivoFinRonda { Eliminacion, Tiempo, Desactivacion, Detonacion }
public enum TipoEquipo { AntiTerrorista, Terrorista, Ninguno }
public enum MatchState { Warmup, BuyTime, InProgress, BombPlanted, RoundEnd, MatchFinished }

public class MatchManager : NetworkBehaviour
{
    public static MatchManager Instance { get; private set; }

    [Header("Estados de Partida")]
    [Networked] public MatchState EstadoActual { get; set; }

    [Header("Variables de HUD y Victoria")]
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

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            EstadoActual = MatchState.Warmup;
            TiempoRestante = 5f;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if (TiempoRestante > 0)
        {
            TiempoRestante -= Runner.DeltaTime;

            if (TiempoRestante <= 0)
            {
                TiempoRestante = 0;
                AvanzarEstadoAutomatico();
            }
        }
    }

    private void AvanzarEstadoAutomatico()
    {
        switch (EstadoActual)
        {
            case MatchState.Warmup:
                IniciarNuevaRonda();
                break;

            case MatchState.BuyTime:
                EstadoActual = MatchState.InProgress;
                TiempoRestante = 115f;
                break;

            case MatchState.InProgress:
                FinalizarRondaExterna(Team.Police, TipoEquipo.AntiTerrorista, MotivoFinRonda.Tiempo);
                break;

            case MatchState.BombPlanted:
                FinalizarRondaExterna(Team.Terrorist, TipoEquipo.Terrorista, MotivoFinRonda.Detonacion, jugadorQuePlanto);
                break;

            case MatchState.RoundEnd:
                IniciarNuevaRonda();
                break;
        }
    }

    public void IniciarNuevaRonda()
    {
        EstadoActual = MatchState.BuyTime;
        TiempoRestante = 15f;
        bombaPlantada = false;
        jugadorQuePlanto = default;

        Debug.Log(" MATCH: Arranca nueva ronda - Fase de Compra y Teletransporte");

       
        ConfiguracionJugadorRed[] jugadores = FindObjectsByType<ConfiguracionJugadorRed>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (ConfiguracionJugadorRed jugador in jugadores)
        {
            if (jugador.HasStateAuthority)
            {
                
                jugador.TeletransportarAlSpawn();

               
                NetworkTransform nt = jugador.GetComponent<NetworkTransform>();
                if (nt != null)
                {
                    nt.Teleport(jugador.transform.position, jugador.transform.rotation);
                }

                
                SaludJugadorRed salud = jugador.GetComponent<SaludJugadorRed>();
                if (salud != null)
                {
                    salud.RestaurarVidaAlMaximo();
                }
            }
        }
    }

    public void AvisarBombaPlantada(PlayerRef planter = default)
    {
        if (!HasStateAuthority) return;
        bombaPlantada = true;
        jugadorQuePlanto = planter;

        EstadoActual = MatchState.BombPlanted;
        TiempoRestante = 40f;

        Debug.Log($" MATCH: Bomba plantada. 40 segundos para la detonación.");
    }

    public void FinalizarRondaExterna(Team equipoGanadorHUD, TipoEquipo equipoEconomia, MotivoFinRonda motivo, PlayerRef jugadorEspecial = default)
    {
        if (!HasStateAuthority) return;
        if (EstadoActual == MatchState.RoundEnd || EstadoActual == MatchState.MatchFinished) return;

        EstadoActual = MatchState.RoundEnd;
        TiempoRestante = 7f;
        UltimoGanador = equipoGanadorHUD;

        if (equipoGanadorHUD == Team.Police) PuntajePolicia++;
        else if (equipoGanadorHUD == Team.Terrorist) PuntajeTerro++;

        FinalizarRondaEconomia(equipoEconomia, motivo, jugadorEspecial);
    }

    private void FinalizarRondaEconomia(TipoEquipo equipoGanador, MotivoFinRonda motivo, PlayerRef jugadorEspecial = default)
    {
        EconomiaJugador[] todasLasEconomias = FindObjectsByType<EconomiaJugador>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (EconomiaJugador eco in todasLasEconomias)
        {
            PlayerRef jugadorRef = eco.Object.InputAuthority;
            ConfiguracionJugadorRed config = eco.GetComponent<ConfiguracionJugadorRed>();
            if (config == null) continue;

            TipoEquipo bandoJugador = TipoEquipo.Ninguno;
            string nombreTeam = config.miEquipo.ToString().ToLower();

            if (nombreTeam.Contains("anti") || nombreTeam.Contains("counter") || nombreTeam == "ct" || nombreTeam.Contains("policia") || config.miEquipo == Team.Police)
                bandoJugador = TipoEquipo.AntiTerrorista;
            else if (nombreTeam.Contains("terror") || nombreTeam == "t" || config.miEquipo == Team.Terrorist)
                bandoJugador = TipoEquipo.Terrorista;

            int plataOtorgada = 0;

            if (equipoGanador == TipoEquipo.AntiTerrorista)
            {
                if (bandoJugador == TipoEquipo.AntiTerrorista)
                {
                    if (motivo == MotivoFinRonda.Eliminacion || motivo == MotivoFinRonda.Tiempo) plataOtorgada = 3250;
                    else if (motivo == MotivoFinRonda.Desactivacion) { plataOtorgada = 3500; if (jugadorRef == jugadorEspecial) plataOtorgada += 300; }
                }
                else if (bandoJugador == TipoEquipo.Terrorista)
                {
                    if (!bombaPlantada) plataOtorgada = 0; else plataOtorgada = 800;
                }
            }
            else if (equipoGanador == TipoEquipo.Terrorista)
            {
                if (bandoJugador == TipoEquipo.Terrorista)
                {
                    if (motivo == MotivoFinRonda.Eliminacion) plataOtorgada = 3250;
                    else if (motivo == MotivoFinRonda.Detonacion) { plataOtorgada = 3500; if (jugadorRef == jugadorQuePlanto) plataOtorgada += 300; }
                }
                else if (bandoJugador == TipoEquipo.AntiTerrorista)
                {
                    plataOtorgada = 1400;
                }
            }

            if (plataOtorgada > 0) eco.RPC_SincronizarPremioRonda(plataOtorgada);
        }
    }
}