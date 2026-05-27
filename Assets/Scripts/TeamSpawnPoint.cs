using UnityEngine;
public enum Team { Police, Terrorist }

public class TeamSpawnPoint : MonoBehaviour
{
    [Header("¿De qué equipo es este spawn?")]
    public Team team;

    
    private void OnDrawGizmos()
    {
        Gizmos.color = team == Team.Police ? Color.blue : Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);

        
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
    }
}