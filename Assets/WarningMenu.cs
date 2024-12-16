using UnityEngine;
using DG.Tweening;
using System.Collections;

public class WarningMenu : MonoBehaviour
{
    public PlayerSettings playerSettings;
    
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Settings")]
    [SerializeField] private float delay = 1f;
    [SerializeField] private float pause = 1f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Ease ease;
    
    private void Start()
    {
        if (!playerSettings.Startup)
        {
            Destroy(gameObject);
        }
        playerSettings.Startup = false;
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, fadeDuration).SetEase(ease).OnComplete(() => StartCoroutine(Delay())); 
    }
    
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        canvasGroup.DOFade(0, fadeDuration).SetEase(ease).OnComplete(() => StartCoroutine(Pause()));
    }
    
    private IEnumerator Pause()
    {
        yield return new WaitForSeconds(pause);
        Destroy(gameObject);
    }
    
}
