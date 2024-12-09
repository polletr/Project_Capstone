using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBatteryUIHandler : Singleton<PlayerBatteryUIHandler>
{
    [Header("References")] public GameEvent Event;
    [SerializeField] private FlashLight flashLight;

    [SerializeField] private Image batteryImage;
    [SerializeField] private Image chargeImage;
    [Header("Settings")] [SerializeField] private float smoothTime = 2f;
    [SerializeField] private float blinkSpeed = 0.5f;

    [SerializeField] private Color batteryLowColor = Color.red;
    [SerializeField] private Color normalColor = Color.black;

    private float BatteryCharge => flashLight.BatteryLife;

    private bool isBlinking;

    private void Start()
    {
        if (flashLight == null) Debug.LogError("Flashlight is null on PlayerBatteryUIHandler");
        if (batteryImage == null) Debug.LogError("BatteryImage is null on PlayerBatteryUIHandler");
        if (chargeImage == null) Debug.LogError("ChargeImage is null on PlayerBatteryUIHandler");
        if (Event == null) Debug.LogError("Event is null on PlayerBatteryUIHandler");


        batteryImage.gameObject.SetActive(flashLight.gameObject.activeSelf);
        batteryImage.color = chargeImage.color = normalColor;
    }

    private void OnEnable()
    {
        Event.OnPickupFlashlight += TurnOnBatteryUI;
    }

    private void OnDisable()
    {
        Event.OnPickupFlashlight -= TurnOnBatteryUI;
    }

    private void Update()
    {
        if (!flashLight.gameObject.activeSelf || !flashLight)
        {
            batteryImage.gameObject.SetActive(false);
            return;
        }

        batteryImage.color = BatteryCharge <= 0 ? batteryLowColor : normalColor;
        var charge = BatteryCharge / flashLight.TotalBatteryLife;
        chargeImage.fillAmount = Mathf.Lerp(chargeImage.fillAmount, charge, Time.deltaTime * smoothTime);

        if (BatteryCharge <= 0 && !isBlinking)
        {
            StartCoroutine(Blink());
        }
    }

    private IEnumerator Blink(bool blinkOnce = false)
    {
        var count = 0;
        isBlinking = true;
        while (BatteryCharge <= 0 || blinkOnce)
        {
            batteryImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkSpeed);
            batteryImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkSpeed);
            count++;
            blinkOnce = count < 3;
        }

        batteryImage.gameObject.SetActive(true);
        isBlinking = false;
    }

    private void TurnOnBatteryUI()
    {
        batteryImage.gameObject.SetActive(true);
    }

    public void FlickerBatteryUIOnce()
    {
        if (!isBlinking)
        {
            StartCoroutine(Blink(true));
        }
    }
}