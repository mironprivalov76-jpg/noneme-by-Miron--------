using UnityEngine;

public class Block : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public int scoreValue = 5;
    public GameObject destroyEffect;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public bool Damage(int amount)
    {
        currentHealth -= amount;

        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, Quaternion.identity);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}