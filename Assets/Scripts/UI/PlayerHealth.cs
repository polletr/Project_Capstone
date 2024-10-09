using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    private float _maxHealth;

    public void SetHealth(float health)
    {
    }

    public void SetMaxHealth(float maxHealth)
    {
        _maxHealth = Mathf.Max(0, maxHealth); // Ensure max health is positive
    }
}
