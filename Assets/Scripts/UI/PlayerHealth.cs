using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
   [field: SerializeField] public Volume volume { get; set;}
    private Vignette vignette;


    private void Awake()
    {
        volume.profile.TryGet(out vignette);
    }

    public void SetHealth(float health)
    {
        vignette.intensity.value = 1 - health / 100;
    }
}
