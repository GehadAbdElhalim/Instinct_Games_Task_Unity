using UnityEngine;
using UnityEngine.Events;

public class HealthBehaviour : MonoBehaviour
{
    public static UnityEvent OnPlayerDead = new UnityEvent();
    public float maxHealth;
    float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(float amount)
    {
        if(currentHealth <= 0)
        {
            return;
        }

        currentHealth -= maxHealth;

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            OnPlayerDead.Invoke();
        }
    }
}
