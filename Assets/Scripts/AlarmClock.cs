using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMOD.Studio;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Serialization;

public class AlarmClock : MonoBehaviour
{
    [Header("Alarm Clock")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float alarmMoveSpeed = 0.3f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 0.5f;
    [SerializeField] private Ease ease = Ease.Linear;
    [SerializeField] private float dissolveDelay = 2f;
    
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private DissolveEffectOnObject dissolve;
    [SerializeField] private AudioHandler ringAudio;
    [SerializeField] private AudioHandler dropAudio;

    private Tween ringingTween;
    private Tween riseTween;
    private Tween turnTween;
    private Sequence sequence;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var triggerBox = GetComponent<Collider>();
        triggerBox.enabled = false;
        
        sequence = DOTween.Sequence();
        sequence
            .Append(riseTween = transform.DOMoveY(transform.position.y + jumpHeight, alarmMoveSpeed).SetEase(ease))
            .Append(ringingTween = transform.DOShakeRotation(shakeDuration, shakeStrength, 10, 5f, false).SetEase(ease));
            
        
        riseTween.OnComplete(() =>
        {
            ringAudio.Play3DAudio(10f);
        });
        
        ringingTween.OnComplete(() =>
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddTorque(Vector3.right, ForceMode.Impulse);
            ringAudio.StopAudioInstance();
        });
     
        sequence.Play();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) return;
        dropAudio.PlayOneShotAudio(transform);
        StartCoroutine(DissolveDely());
    }

    private IEnumerator DissolveDely()
    {
        yield return new WaitForSeconds(dissolveDelay);
        dissolve.DisableObj();
    }

    private void OnDestroy()
    {
        riseTween?.Kill();
        ringingTween?.Kill();
    }
}