using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowEnemyBlink : EnemyClass, IStunable
{
    [field: SerializeField] public Material BodyMaterial { get; private set; }
    [field: SerializeField] public Material EyeMaterial { get; private set; }
    public EventInstance currentAudio { get; set; }

    [SerializeField] private float changeTranspDuration;

    private Coroutine EnemyTransparencyRoutine;

    private void Awake()
    {
        BodyMaterial.SetFloat("_Transparency", 0.9f);
        EyeMaterial.SetFloat("_Transparency", 0.9f);
        currentAudio = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.ShadowIdle);
        currentAudio.start();
    }

    public void ApplyEffect()
    {
        if (EnemyTransparencyRoutine == null)
            EnemyTransparencyRoutine = StartCoroutine(EnemyTransparency(0f));
    }

    public void ApplyStunEffect()
    {

    }

    public void RemoveEffect()
    {

    }


    public IEnumerator EnemyTransparency(float targetTransp)
    {
        // Get current transparency (alpha) values from the body and eye materials
        float bodyCurrentTransparency = BodyMaterial.GetFloat("_Transparency");
        float eyeCurrentTransparency = EyeMaterial.GetFloat("_Transparency");

        // Store the starting values for interpolation
        float startBodyTransparency = bodyCurrentTransparency;
        float startEyeTransparency = eyeCurrentTransparency;

        float elapsedTime = 0f;

        // Gradually change transparency over the duration
        while (elapsedTime < changeTranspDuration)
        {
            // Calculate the new transparency using Lerp
            float newBodyTransparency = Mathf.Lerp(startBodyTransparency, targetTransp, elapsedTime / changeTranspDuration);
            float newEyeTransparency = Mathf.Lerp(startEyeTransparency, targetTransp, elapsedTime / changeTranspDuration);

            // Set the new transparency values back to the materials
            BodyMaterial.SetFloat("_Transparency", newBodyTransparency);
            EyeMaterial.SetFloat("_Transparency", newEyeTransparency);

            // Increase the elapsed time
            elapsedTime += Time.deltaTime;

            // Yield until the next frame
            yield return null;
        }

        // Ensure the transparency is set to the target value at the end
        BodyMaterial.SetFloat("_Transparency", targetTransp);
        EyeMaterial.SetFloat("_Transparency", targetTransp);
        DisableEnemy();
    }

    public void DisableEnemy()
    {
        currentAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        this.gameObject.SetActive(false);
    }

}
