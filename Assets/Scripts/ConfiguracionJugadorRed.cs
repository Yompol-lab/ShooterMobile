using Fusion;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

public class ConfiguracionJugadorRed : NetworkBehaviour
{
    [Header("Componentes a apagar en los RIVALES")]
    public Camera camaraDelJugador;
    public AudioListener audioListener;
    public PlayerInput playerInput;
    public FirstPersonController controladorMovimiento;

    [Header("Scripts para conectar a MIS controles")]
    public StarterAssetsInputs misInputs;
    public PlayerWeaponController miArma;
    public PlayerInventory miInventario;

    [Header("Datos de Partida")]
    [Networked] public Team miEquipo { get; set; }
    [Networked] public NetworkBool tieneBomba { get; set; }

    // NUEVO
    [Networked]
    public NetworkString<_16> nombreJugador { get; set; }

    [Networked]
    public byte avatarID { get; set; }

    [Header("Configuración de Bomba")]
    public GameObject modeloBombaEnMano; 

    public void TeletransportarAlSpawn()
    {
        TeamSpawnPoint[] todosLosSpawns = FindObjectsByType<TeamSpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var spawnsValidos = System.Array.FindAll(todosLosSpawns, sp => sp.team == miEquipo);

        if (spawnsValidos.Length > 0)
        {
            int rand = Random.Range(0, spawnsValidos.Length);
            Vector3 nuevaPosicion = spawnsValidos[rand].transform.position;
            Quaternion nuevaRotacion = spawnsValidos[rand].transform.rotation;

           
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            transform.position = nuevaPosicion;
            transform.rotation = nuevaRotacion;

            if (cc != null) cc.enabled = true;
        }
    }


    private void Update()
    {
       
        if (HasStateAuthority && misInputs != null && MatchManager.Instance != null)
        {
            if (MatchManager.Instance.EstadoActual == MatchState.BuyTime)
            {
                
                misInputs.move = Vector2.zero;
                misInputs.jump = false;
                misInputs.sprint = false;
            }
        }
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Debug.Log("Cargando perfil...");
            nombreJugador = PlayerPrefs.GetString("NombreJugador", "Jugador");
            avatarID = (byte)PlayerPrefs.GetInt("AvatarID", 0);

            MobileControlsBridge mobileControls = FindFirstObjectByType<MobileControlsBridge>();

            if (mobileControls != null)
            {
                mobileControls.starterInputs = misInputs;
                mobileControls.weaponController = miArma;
                mobileControls.playerInventory = miInventario;
                mobileControls.jugadorLocal = this;
            }
        }
        else
        {
            if (camaraDelJugador != null)
                camaraDelJugador.gameObject.SetActive(false);

            if (audioListener != null)
                audioListener.enabled = false;

            if (playerInput != null)
                playerInput.enabled = false;

            if (controladorMovimiento != null)
                controladorMovimiento.enabled = false;
        }

      
    }

   

    public void IntentarEquiparBomba()
    {
       
        if (HasStateAuthority && tieneBomba)
        {
            
            bool sacarBomba = !modeloBombaEnMano.activeSelf;

           
            RPC_AlternarBomba(sacarBomba);
        }
    }

    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AlternarBomba(bool estadoBomba)
    {
        if (modeloBombaEnMano != null) modeloBombaEnMano.SetActive(estadoBomba);

        
        if (miArma != null) miArma.gameObject.SetActive(!estadoBomba);
    }


}