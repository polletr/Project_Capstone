using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowEnemyBlink : EnemyClass, IEffectable
{
    [SerializeField] private Material BodyMaterial;
    [SerializeField] private Material EyeMaterial;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private float startTransparency;
    public EventInstance currentAudio { get; set; }

    [SerializeField] private float changeTranspDuration;

    private Coroutine EnemyTransparencyRoutine;

    private PlayerController playerCharacter;

    public bool CanApplyEffect { get; set; }

    private void Awake()
    {
        playerCharacter = Object.FindAnyObjectByType<PlayerController>();
        currentAudio = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.ShadowIdle);
        // 3D attributes based on the enemy's position and orientation
        FMOD.ATTRIBUTES_3D attributes = new FMOD.ATTRIBUTES_3D
        {
            position = RuntimeUtils.ToFMODVector(transform.position),
            forward = RuntimeUtils.ToFMODVector(transform.forward),
            up = RuntimeUtils.ToFMODVector(transform.up)
        };

        currentAudio.set3DAttributes(attributes);
        currentAudio.start();
        CanApplyEffect = true;
        SetTransparency(startTransparency);

    }

    private void Update()
    {
        RotateToPlayer();
    }

    public void ApplyEffect()
    {
        SetTransparency(0f);
    }

    public void ApplyStunEffect()
    {

    }

    public void RemoveEffect()
    {

    }

    public void SetTransparency(float value)
    {
        if (EnemyTransparencyRoutine == null)
            EnemyTransparencyRoutine = StartCoroutine(EnemyTransparency(value));
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
        if (bodyCurrentTransparency <= 0f)
        {
            DisableEnemy();
        }
        EnemyTransparencyRoutine = null;
    }

    public void DisableEnemy()
    {
        currentAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        this.gameObject.SetActive(false);
    }

    protected virtual void RotateToPlayer()
    {
        // Get the direction to the player
        Vector3 directionToPlayer =
            (playerCharacter.transform.position - transform.position).normalized;

        Quaternion lookRotation =
            Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0,
                directionToPlayer.z)); // Ignore y-axis to keep rotation flat

        // Smoothly rotate the enemy towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation,
            Time.deltaTime * RotationSpeed);

    }


}
