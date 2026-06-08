using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugAuthority : NetworkBehaviour
{
    public override void Spawned()
    {
        Debug.LogError(
            "AUTHORITY => Input: " + HasInputAuthority +
            " | State: " + HasStateAuthority
        );

        if (!HasInputAuthority)
        {
            Camera cam = GetComponentInChildren<Camera>(true);
            if (cam != null)
                cam.enabled = false;

            PlayerInput input = GetComponent<PlayerInput>();
            if (input != null)
                input.enabled = false;
        }
    }
}