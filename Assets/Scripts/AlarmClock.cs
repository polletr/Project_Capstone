using UnityEngine;
using DG.Tweening;

public class AlarmClock : MonoBehaviour
{
    [Header("Alarm Clock")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float alarmMoveSpeed = 0.3f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 0.5f;
    [SerializeField] private Ease ease = Ease.Linear;

    private Tween shakeTween;
    private Tween ringTween;
    private Sequence sequence;
  
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var triggerBox = GetComponent<Collider>();
        triggerBox.enabled = false;
        
        sequence = DOTween.Sequence();
        sequence
            .Append(ringTween = transform.DOMoveY(transform.position.y + jumpHeight, alarmMoveSpeed).SetEase(ease))
            .Append(shakeTween = transform.DOShakeRotation(shakeDuration, shakeStrength).SetEase(ease));
        
        shakeTween.OnComplete(() =>
        {
           //AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.RingingAlarm, transform.position);
           triggerBox.enabled = true;
        });
        sequence.Play();
    }

    private void OnDestroy()
    {
        ringTween?.Kill();
        shakeTween?.Kill();
    }
}