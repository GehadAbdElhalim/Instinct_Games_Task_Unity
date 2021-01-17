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
        GetComponent<MeshRenderer>().material.color = new Color(1, currentHealth / maxHealth, currentHealth / maxHealth);
    }

    [ContextMenu("Damage")]
    void Damage10()
    {
        Damage(10);
    }

    public void Damage(float amount)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth -= amount;
        GetComponent<MeshRenderer>().material.color = new Color(1, currentHealth / maxHealth, currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnPlayerDead.Invoke();
        }
    }
}
