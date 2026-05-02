using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public float speed = 5f;

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            transform.Translate(new Vector3(x, 0, z) * speed * Runner.DeltaTime);
        }
    }
}
