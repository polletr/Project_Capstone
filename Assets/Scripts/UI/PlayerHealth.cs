using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    public Volume volume;
    public AnimationCurve healthToVignetteCurve; // Use this curve to map health to vignette intensity

    [SerializeField] private float _speed = 1.0f;

    private Vignette _vignette;
    private float _maxHealth;

    private void Awake()
    {
        volume.profile.TryGet(out _vignette);
    }

    public void SetHealth(float health)
    {
        if (_vignette != null)
        {
            float normalizedHealth = Mathf.Clamp01(health / _maxHealth);
            _vignette.intensity.value = healthToVignetteCurve.Evaluate(normalizedHealth);
        }
    }

    public void SetMaxHealth(float maxHealth)
    {
        _maxHealth = Mathf.Max(0, maxHealth); // Ensure max health is positive
    }
}
