using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Configuración del Efecto")]
    [Tooltip("Arrastrá acá tu prefab Fog_Dissolve")]
    public GameObject smokePrefab;
    public int numberOfSmokes = 3;
    public float spreadRadius = 0.5f; 

    private bool hasExploded = false;

    
    public void TriggerSmoke()
    {
        
        if (hasExploded) return;
        hasExploded = true;

        for (int i = 0; i < numberOfSmokes; i++)
        {
            
            Vector3 randomOffset = new Vector3(
                Random.Range(-spreadRadius, spreadRadius),
                Random.Range(0, spreadRadius), 
                Random.Range(-spreadRadius, spreadRadius)
            );

            Vector3 spawnPosition = transform.position + randomOffset;

            
            Instantiate(smokePrefab, spawnPosition, Quaternion.identity);
        }

    }
}