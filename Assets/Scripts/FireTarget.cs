using UnityEngine;

public class FireTarget : MonoBehaviour
{
    public void Extinguish()
    {
        
        if (transform.parent != null)
        {
            
            Destroy(transform.parent.gameObject);
        }
        else
        {
            
            Destroy(gameObject);
        }
    }
}