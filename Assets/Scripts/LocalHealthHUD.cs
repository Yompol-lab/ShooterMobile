using Fusion;
using UnityEngine;
using TMPro;

public class LocalHealthHUD : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    private Health localPlayerHealth;

    private void Update()
    {
        
        if (localPlayerHealth == null)
        {
            Health[] allHealths = FindObjectsByType<Health>(FindObjectsSortMode.None);
            foreach (Health h in allHealths)
            {
                
                if (h.Object != null && h.Object.HasInputAuthority)
                {
                    localPlayerHealth = h;
                    break; 
                }
            }
        }

        
        if (localPlayerHealth != null)
        {
            int currentHP = Mathf.CeilToInt(localPlayerHealth.currentHealth);

            
            if (currentHP < 0) currentHP = 0;

            healthText.text = currentHP.ToString();

            
            if (currentHP <= 20)
            {
                healthText.color = Color.red;
            }
            else
            {
                healthText.color = Color.white;
            }

            
            if (localPlayerHealth.isDead)
            {
                healthText.text = "0";
            }
        }
        else
        {
            
            healthText.text = "";
        }
    }
}