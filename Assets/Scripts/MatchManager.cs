using Fusion;
using UnityEngine;
using System.Linq;

public enum MatchState { Warmup, BuyTime, Playing, BombPlanted, RoundEnd, MatchFinished }

public class MatchManager : NetworkBehaviour
{
    public static MatchManager Instance;

    [Header("Configuración de Tiempos")]
    public float tiempoCalentamiento = 15f;
    public float tiempoCompra = 15f;
    public float tiempoRonda = 120f;
    public float tiempoBomba = 45f;
    public float tiempoFinRonda = 10f;
    public int rondasParaGanar = 5;

    [Header("Prefabs del Juego")]
    public NetworkPrefabRef prefabBombaC4;

    [Header("Variables de Red (No tocar)")]
    [Networked] public MatchState EstadoActual { get; set; }
    [Networked] public float TiempoRestante { get; set; }
    [Networked] public int PuntajePolicia { get; set; }
    [Networked] public int PuntajeTerro { get; set; }
    [Networked] public int RondaActual { get; set; }

    [Networked] public Team UltimoGanador { get; set; }

    public override void Spawned()
    {
        Instance = this;
        if (Runner.IsSharedModeMasterClient)
        {
            Object.RequestStateAuthority();
            IniciarCalentamiento();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority || EstadoActual == MatchState.MatchFinished) return;

        TiempoRestante -= Runner.DeltaTime;

        if (TiempoRestante <= 0)
        {
            if (EstadoActual == MatchState.Warmup) IniciarTiempoCompra();
            else if (EstadoActual == MatchState.BuyTime) IniciarRonda();
            else if (EstadoActual == MatchState.Playing) TerminarRonda(Team.Police);
            else if (EstadoActual == MatchState.BombPlanted) TerminarRonda(Team.Terrorist);
            else if (EstadoActual == MatchState.RoundEnd) IniciarTiempoCompra();
        }
    }

    public void AvisarBombaPlantada()
    {
        if (EstadoActual == MatchState.Playing)
        {
            EstadoActual = MatchState.BombPlanted;
            TiempoRestante = tiempoBomba;
        }
    }

    public void IniciarCalentamiento() { EstadoActual = MatchState.Warmup; TiempoRestante = tiempoCalentamiento; PuntajePolicia = 0; PuntajeTerro = 0; RondaActual = 0; }
    public void IniciarTiempoCompra() { EstadoActual = MatchState.BuyTime; TiempoRestante = tiempoCompra; RondaActual++; RPC_ReiniciarJugadores(); RepartirBomba(); }
    public void IniciarRonda() { EstadoActual = MatchState.Playing; TiempoRestante = tiempoRonda; }

    public void TerminarRonda(Team equipoGanador)
    {
        EstadoActual = MatchState.RoundEnd;
        TiempoRestante = tiempoFinRonda;

        UltimoGanador = equipoGanador;

        if (equipoGanador == Team.Police) PuntajePolicia++; else PuntajeTerro++;
        if (PuntajePolicia >= rondasParaGanar || PuntajeTerro >= rondasParaGanar) EstadoActual = MatchState.MatchFinished;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ReiniciarJugadores()
    {
        ConfiguracionJugadorRed miJugador = FindObjectsByType<ConfiguracionJugadorRed>(FindObjectsSortMode.None).FirstOrDefault(j => j.HasStateAuthority);
        if (miJugador != null) miJugador.TeletransportarAlSpawn();
    }

    private void RepartirBomba()
    {
        var terroristas = FindObjectsByType<ConfiguracionJugadorRed>(FindObjectsSortMode.None).Where(j => j.miEquipo == Team.Terrorist).ToList();
        if (terroristas.Count > 0)
        {
            int elegido = Random.Range(0, terroristas.Count);
            ConfiguracionJugadorRed terroElegido = terroristas[elegido];
            Runner.Spawn(prefabBombaC4, terroElegido.transform.position + Vector3.up, Quaternion.identity, terroElegido.Object.InputAuthority);
        }
    }
}